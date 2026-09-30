using Moq;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class ExpenseServiceTests
{
    private readonly Mock<IExpenseRepository> _expenseRepository = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly ExpenseService _sut;

    public ExpenseServiceTests()
    {
        _sut = new ExpenseService(_expenseRepository.Object, _accountRepository.Object);
    }

    [Fact]
    public async Task RegisterExpenseAsync_BlocksWhenAmountExceedsBalance()
    {
        var accountId = Guid.NewGuid();
        _accountRepository.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(new Account { Id = accountId, Name = "BBVA", Balance = 100m });

        var result = await _sut.RegisterExpenseAsync(accountId, "Comida", 500m);

        Assert.False(result.Success);
        Assert.Contains("saldo", result.Error, StringComparison.OrdinalIgnoreCase);
        _accountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
        _expenseRepository.Verify(r => r.AddAsync(It.IsAny<Expense>()), Times.Never);
    }

    [Fact]
    public async Task RegisterExpenseAsync_DebitsExactAmountWhenBalanceIsSufficient()
    {
        var accountId = Guid.NewGuid();
        _accountRepository.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(new Account { Id = accountId, Name = "BBVA", Balance = 1000m });
        _accountRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Account> { new() { Id = accountId, Balance = 700m } });

        var result = await _sut.RegisterExpenseAsync(accountId, "Comida", 300m);

        Assert.True(result.Success);
        Assert.Equal(700m, result.AccountBalance);
        _accountRepository.Verify(r => r.UpdateBalanceAsync(accountId, 700m), Times.Once);
        _expenseRepository.Verify(r => r.AddAsync(It.Is<Expense>(e => e.AccountId == accountId && e.Amount == 300m)), Times.Once);
    }

    [Fact]
    public async Task RegisterExpenseAsync_FailsWhenAccountDoesNotExist()
    {
        var accountId = Guid.NewGuid();
        _accountRepository.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Account?)null);

        var result = await _sut.RegisterExpenseAsync(accountId, "Comida", 100m);

        Assert.False(result.Success);
        _accountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task RegisterExpenseAsync_RejectsZeroOrNegativeAmount()
    {
        var result = await _sut.RegisterExpenseAsync(Guid.NewGuid(), "Comida", 0m);

        Assert.False(result.Success);
        _accountRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}
