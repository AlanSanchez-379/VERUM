using Moq;
using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class CollectionServiceTests
{
    private readonly Mock<IReceivableRepository> _receivableRepository = new();
    private readonly Mock<ISaleService> _saleService = new();
    private readonly Mock<IBusinessAccountService> _accountService = new();
    private readonly CollectionService _sut;
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _accountId = Guid.NewGuid();

    public CollectionServiceTests()
    {
        _sut = new CollectionService(_receivableRepository.Object, _saleService.Object, _accountService.Object);
        _accountService.Setup(s => s.GetAllAsync(_businessId))
            .ReturnsAsync(new List<BusinessAccountDto> { new(_accountId, _businessId, "Caja", "", 0m) });
    }

    [Fact]
    public async Task MarkCollectedAsync_AlreadyCollected_DoesNotRegisterSaleAgain()
    {
        var receivableId = Guid.NewGuid();
        _receivableRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Receivable>
        {
            new() { Id = receivableId, BusinessId = _businessId, ClientName = "Juan", Amount = 500m, IsCollected = true }
        });

        var result = await _sut.MarkCollectedAsync(_businessId, receivableId, _accountId);

        Assert.False(result.Success);
        _saleService.Verify(s => s.RegisterSaleAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task MarkCollectedAsync_RequiresAnAccount()
    {
        var result = await _sut.MarkCollectedAsync(_businessId, Guid.NewGuid(), Guid.Empty);

        Assert.False(result.Success);
        _receivableRepository.Verify(r => r.GetAllAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task MarkCollectedAsync_PropagatesDescriptionIntoTheGeneratedSale()
    {
        var receivableId = Guid.NewGuid();
        _receivableRepository.Setup(r => r.GetAllAsync(_businessId)).ReturnsAsync(new List<Receivable>
        {
            new() { Id = receivableId, BusinessId = _businessId, ClientName = "Juan", Description = "Reparacion laptop", Amount = 500m, IsCollected = false }
        });

        var result = await _sut.MarkCollectedAsync(_businessId, receivableId, _accountId);

        Assert.True(result.Success);
        _saleService.Verify(s => s.RegisterSaleAsync(_businessId, _accountId, It.Is<string>(d => d.Contains("Reparacion laptop")), 500m), Times.Once);
        _receivableRepository.Verify(r => r.MarkCollectedAsync(_businessId, receivableId), Times.Once);
    }
}
