using Moq;
using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class RecurringIncomeServiceTests
{
    private readonly Mock<IRecurringIncomeRepository> _recurringIncomeRepository = new();
    private readonly Mock<IIncomeService> _incomeService = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly RecurringIncomeService _sut;

    public RecurringIncomeServiceTests()
    {
        _sut = new RecurringIncomeService(_recurringIncomeRepository.Object, _incomeService.Object, _accountRepository.Object);
        _accountRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Account>());
    }

    [Fact]
    public async Task ConfirmAsync_SamePeriodTwice_RejectsSecondConfirmation()
    {
        var patternId = Guid.NewGuid();
        _recurringIncomeRepository.Setup(r => r.GetConfirmationsForPeriodAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RecurringIncomeConfirmation> { new() { RecurringIncomeId = patternId } });

        var result = await _sut.ConfirmAsync(patternId, Guid.NewGuid(), 500m);

        Assert.False(result.Success);
        _incomeService.Verify(s => s.RegisterIncomeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _recurringIncomeRepository.Verify(r => r.AddConfirmationAsync(It.IsAny<RecurringIncomeConfirmation>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmAsync_ZeroAmount_RecordsConfirmationButMovesNoMoney()
    {
        var patternId = Guid.NewGuid();
        _recurringIncomeRepository.Setup(r => r.GetConfirmationsForPeriodAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RecurringIncomeConfirmation>());

        var result = await _sut.ConfirmAsync(patternId, null, 0m);

        Assert.True(result.Success);
        _incomeService.Verify(s => s.RegisterIncomeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _recurringIncomeRepository.Verify(r => r.AddConfirmationAsync(It.Is<RecurringIncomeConfirmation>(c => c.ConfirmedAmount == 0m && c.IncomeId == null)), Times.Once);
    }

    [Fact]
    public async Task ConfirmAsync_PositiveAmountWithoutAccount_Rejects()
    {
        var patternId = Guid.NewGuid();
        _recurringIncomeRepository.Setup(r => r.GetConfirmationsForPeriodAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RecurringIncomeConfirmation>());

        var result = await _sut.ConfirmAsync(patternId, null, 500m);

        Assert.False(result.Success);
        _incomeService.Verify(s => s.RegisterIncomeAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmAsync_HappyPath_RegistersIncomeWithActualAmountAndConfirms()
    {
        var patternId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        _recurringIncomeRepository.Setup(r => r.GetConfirmationsForPeriodAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RecurringIncomeConfirmation>());
        _recurringIncomeRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RecurringIncome>
        {
            new() { Id = patternId, Source = "Sueldo", ExpectedAmount = 4000m, DayOfMonth = 5, IsActive = true }
        });
        _incomeService.Setup(s => s.RegisterIncomeAsync(accountId, "Sueldo", 3500m))
            .ReturnsAsync(new RegisterIncomeResult(true, null, 10000m, 3500m));

        var result = await _sut.ConfirmAsync(patternId, accountId, 3500m);

        Assert.True(result.Success);
        _incomeService.Verify(s => s.RegisterIncomeAsync(accountId, "Sueldo", 3500m), Times.Once);
        _recurringIncomeRepository.Verify(r => r.AddConfirmationAsync(It.Is<RecurringIncomeConfirmation>(c => c.RecurringIncomeId == patternId && c.ConfirmedAmount == 3500m)), Times.Once);
    }
}
