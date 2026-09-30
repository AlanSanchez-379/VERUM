using Moq;
using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class TaxServiceTests
{
    private readonly Mock<ITaxRepository> _taxRepository = new();
    private readonly Mock<IBusinessExpenseService> _expenseService = new();
    private readonly Mock<IBusinessAccountService> _accountService = new();
    private readonly TaxService _sut;
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _accountId = Guid.NewGuid();

    public TaxServiceTests()
    {
        _sut = new TaxService(_taxRepository.Object, _expenseService.Object, _accountService.Object);
        _accountService.Setup(s => s.GetOrCreateDefaultAccountIdAsync(_businessId)).ReturnsAsync(_accountId);
    }

    [Fact]
    public async Task MarkPaidAsync_DoesNotMarkPaid_WhenExpenseRegistrationFails()
    {
        var taxId = Guid.NewGuid();
        _taxRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Tax>
        {
            new() { Id = taxId, BusinessId = _businessId, Name = "IVA", Amount = 500m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_businessId, _accountId, It.IsAny<string>(), 500m))
            .ReturnsAsync(new BusinessExpenseResult(false, "sin saldo"));

        var result = await _sut.MarkPaidAsync(_businessId, taxId);

        Assert.False(result.Success);
        _taxRepository.Verify(r => r.MarkPaidAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task MarkPaidAsync_MarksPaid_WhenExpenseRegistrationSucceeds()
    {
        var taxId = Guid.NewGuid();
        _taxRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Tax>
        {
            new() { Id = taxId, BusinessId = _businessId, Name = "IVA", Amount = 500m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_businessId, _accountId, It.IsAny<string>(), 500m))
            .ReturnsAsync(new BusinessExpenseResult(true, null));

        var result = await _sut.MarkPaidAsync(_businessId, taxId);

        Assert.True(result.Success);
        _taxRepository.Verify(r => r.MarkPaidAsync(_businessId, taxId), Times.Once);
    }
}
