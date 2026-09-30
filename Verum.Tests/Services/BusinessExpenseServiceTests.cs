using Moq;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class BusinessExpenseServiceTests
{
    private readonly Mock<IBusinessExpenseRepository> _expenseRepository = new();
    private readonly Mock<IBusinessAccountRepository> _accountRepository = new();
    private readonly BusinessExpenseService _sut;
    private readonly Guid _businessId = Guid.NewGuid();

    public BusinessExpenseServiceTests()
    {
        _sut = new BusinessExpenseService(_expenseRepository.Object, _accountRepository.Object);
    }

    [Fact]
    public async Task RegisterExpenseAsync_BlocksWhenAmountExceedsBalance()
    {
        var accountId = Guid.NewGuid();
        _accountRepository.Setup(r => r.GetByIdAsync(_businessId, accountId))
            .ReturnsAsync(new BusinessAccount { Id = accountId, BusinessId = _businessId, Name = "Caja", Balance = 50m });

        var result = await _sut.RegisterExpenseAsync(_businessId, accountId, "Alquiler", 200m);

        Assert.False(result.Success);
        _accountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
        _expenseRepository.Verify(r => r.AddAsync(It.IsAny<BusinessExpense>()), Times.Never);
    }

    [Fact]
    public async Task RegisterExpenseAsync_DebitsExactAmountWhenBalanceIsSufficient()
    {
        var accountId = Guid.NewGuid();
        _accountRepository.Setup(r => r.GetByIdAsync(_businessId, accountId))
            .ReturnsAsync(new BusinessAccount { Id = accountId, BusinessId = _businessId, Name = "Caja", Balance = 1000m });

        var result = await _sut.RegisterExpenseAsync(_businessId, accountId, "Alquiler", 400m);

        Assert.True(result.Success);
        _accountRepository.Verify(r => r.UpdateBalanceAsync(_businessId, accountId, 600m), Times.Once);
        _expenseRepository.Verify(r => r.AddAsync(It.Is<BusinessExpense>(e => e.AccountId == accountId && e.Amount == 400m)), Times.Once);
    }
}
