using Moq;
using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class PayableServiceTests
{
    private readonly Mock<IPayableRepository> _payableRepository = new();
    private readonly Mock<IBusinessExpenseService> _expenseService = new();
    private readonly Mock<IBusinessAccountService> _accountService = new();
    private readonly PayableService _sut;
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _accountId = Guid.NewGuid();

    public PayableServiceTests()
    {
        _sut = new PayableService(_payableRepository.Object, _expenseService.Object, _accountService.Object);
        _accountService.Setup(s => s.GetOrCreateDefaultAccountIdAsync(_businessId)).ReturnsAsync(_accountId);
    }

    [Fact]
    public async Task MarkPaidAsync_DoesNotMarkPaid_WhenExpenseRegistrationFails()
    {
        var payableId = Guid.NewGuid();
        _payableRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Payable>
        {
            new() { Id = payableId, BusinessId = _businessId, SupplierName = "Distribuidora", Amount = 2000m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_businessId, _accountId, It.IsAny<string>(), 2000m))
            .ReturnsAsync(new BusinessExpenseResult(false, "sin saldo"));

        var result = await _sut.MarkPaidAsync(_businessId, payableId);

        Assert.False(result.Success);
        _payableRepository.Verify(r => r.MarkPaidAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task MarkPaidAsync_MarksPaid_WhenExpenseRegistrationSucceeds()
    {
        var payableId = Guid.NewGuid();
        _payableRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Payable>
        {
            new() { Id = payableId, BusinessId = _businessId, SupplierName = "Distribuidora", Amount = 2000m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_businessId, _accountId, It.IsAny<string>(), 2000m))
            .ReturnsAsync(new BusinessExpenseResult(true, null));

        var result = await _sut.MarkPaidAsync(_businessId, payableId);

        Assert.True(result.Success);
        _payableRepository.Verify(r => r.MarkPaidAsync(_businessId, payableId), Times.Once);
    }
}
