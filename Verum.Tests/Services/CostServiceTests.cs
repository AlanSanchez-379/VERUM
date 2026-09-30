using Moq;
using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class CostServiceTests
{
    private readonly Mock<ICostRepository> _costRepository = new();
    private readonly Mock<IBusinessExpenseService> _expenseService = new();
    private readonly Mock<IBusinessAccountService> _accountService = new();
    private readonly CostService _sut;
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _accountId = Guid.NewGuid();

    public CostServiceTests()
    {
        _sut = new CostService(_costRepository.Object, _expenseService.Object, _accountService.Object);
        _accountService.Setup(s => s.GetOrCreateDefaultAccountIdAsync(_businessId)).ReturnsAsync(_accountId);
    }

    [Fact]
    public async Task MarkPaidAsync_DoesNotMarkPaid_WhenExpenseRegistrationFails()
    {
        var costId = Guid.NewGuid();
        _costRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Cost>
        {
            new() { Id = costId, BusinessId = _businessId, Name = "Alquiler", Amount = 1000m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_businessId, _accountId, It.IsAny<string>(), 1000m))
            .ReturnsAsync(new BusinessExpenseResult(false, "No hay suficiente saldo en Caja para este gasto."));

        var result = await _sut.MarkPaidAsync(_businessId, costId);

        Assert.False(result.Success);
        _costRepository.Verify(r => r.MarkPaidAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task MarkPaidAsync_MarksPaid_WhenExpenseRegistrationSucceeds()
    {
        var costId = Guid.NewGuid();
        _costRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Cost>
        {
            new() { Id = costId, BusinessId = _businessId, Name = "Alquiler", Amount = 1000m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_businessId, _accountId, It.IsAny<string>(), 1000m))
            .ReturnsAsync(new BusinessExpenseResult(true, null));

        var result = await _sut.MarkPaidAsync(_businessId, costId);

        Assert.True(result.Success);
        _costRepository.Verify(r => r.MarkPaidAsync(_businessId, costId), Times.Once);
    }

    [Fact]
    public async Task MarkPaidAsync_AlreadyPaid_DoesNotChargeAgain()
    {
        var costId = Guid.NewGuid();
        _costRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Cost>
        {
            new() { Id = costId, BusinessId = _businessId, Name = "Alquiler", Amount = 1000m, IsPaid = true }
        });

        var result = await _sut.MarkPaidAsync(_businessId, costId);

        Assert.False(result.Success);
        _expenseService.Verify(s => s.RegisterExpenseAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
    }
}
