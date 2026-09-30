using Moq;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class TransferServiceTests
{
    private readonly Mock<ITransferRepository> _transferRepository = new();
    private readonly Mock<IAccountRepository> _personalAccountRepository = new();
    private readonly Mock<IBusinessAccountRepository> _businessAccountRepository = new();
    private readonly TransferService _sut;
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _personalAccountId = Guid.NewGuid();
    private readonly Guid _businessAccountId = Guid.NewGuid();

    public TransferServiceTests()
    {
        _sut = new TransferService(_transferRepository.Object, _personalAccountRepository.Object, _businessAccountRepository.Object);
    }

    private void SetupAccounts(decimal personalBalance, decimal businessBalance)
    {
        _personalAccountRepository.Setup(r => r.GetByIdAsync(_personalAccountId))
            .ReturnsAsync(new Account { Id = _personalAccountId, Name = "BBVA", Balance = personalBalance });
        _businessAccountRepository.Setup(r => r.GetByIdAsync(_businessId, _businessAccountId))
            .ReturnsAsync(new BusinessAccount { Id = _businessAccountId, BusinessId = _businessId, Name = "Caja", Balance = businessBalance });
    }

    [Fact]
    public async Task TransferAsync_ToBusiness_BlockedWhenPersonalBalanceInsufficient()
    {
        SetupAccounts(personalBalance: 100m, businessBalance: 0m);

        var result = await _sut.TransferAsync(_businessId, _personalAccountId, _businessAccountId, TransferDirection.ToBusiness, 500m, "");

        Assert.False(result.Success);
        _personalAccountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
        _businessAccountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task TransferAsync_ToPersonal_BlockedWhenBusinessBalanceInsufficient()
    {
        SetupAccounts(personalBalance: 0m, businessBalance: 100m);

        var result = await _sut.TransferAsync(_businessId, _personalAccountId, _businessAccountId, TransferDirection.ToPersonal, 500m, "");

        Assert.False(result.Success);
        _personalAccountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
        _businessAccountRepository.Verify(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task TransferAsync_ToBusiness_MovesExactAmountBothWays()
    {
        SetupAccounts(personalBalance: 1000m, businessBalance: 500m);

        var result = await _sut.TransferAsync(_businessId, _personalAccountId, _businessAccountId, TransferDirection.ToBusiness, 300m, "aporte");

        Assert.True(result.Success);
        _personalAccountRepository.Verify(r => r.UpdateBalanceAsync(_personalAccountId, 700m), Times.Once);
        _businessAccountRepository.Verify(r => r.UpdateBalanceAsync(_businessId, _businessAccountId, 800m), Times.Once);
        _transferRepository.Verify(r => r.AddAsync(It.Is<Transfer>(t => t.Amount == 300m && t.Direction == TransferDirection.ToBusiness)), Times.Once);
    }

    [Fact]
    public async Task TransferAsync_FailsWhenPersonalAccountDoesNotExist()
    {
        _personalAccountRepository.Setup(r => r.GetByIdAsync(_personalAccountId)).ReturnsAsync((Account?)null);

        var result = await _sut.TransferAsync(_businessId, _personalAccountId, _businessAccountId, TransferDirection.ToBusiness, 100m, "");

        Assert.False(result.Success);
        _transferRepository.Verify(r => r.AddAsync(It.IsAny<Transfer>()), Times.Never);
    }
}
