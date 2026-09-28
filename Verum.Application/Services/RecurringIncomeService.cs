using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class RecurringIncomeService : IRecurringIncomeService
{
    private readonly IRecurringIncomeRepository _recurringIncomeRepository;
    private readonly IIncomeService _incomeService;
    private readonly IAccountRepository _accountRepository;

    public RecurringIncomeService(
        IRecurringIncomeRepository recurringIncomeRepository,
        IIncomeService incomeService,
        IAccountRepository accountRepository)
    {
        _recurringIncomeRepository = recurringIncomeRepository;
        _incomeService = incomeService;
        _accountRepository = accountRepository;
    }

    public async Task<List<RecurringIncomeDto>> GetAllAsync()
    {
        var patterns = await _recurringIncomeRepository.GetAllAsync();
        return patterns.Select(ToDto).ToList();
    }

    public async Task RegisterPatternAsync(string source, decimal expectedAmount, int dayOfMonth)
    {
        if (string.IsNullOrWhiteSpace(source) || expectedAmount <= 0 || dayOfMonth is < 1 or > 28)
        {
            return;
        }

        await _recurringIncomeRepository.AddAsync(new RecurringIncome
        {
            Source = source.Trim(),
            ExpectedAmount = expectedAmount,
            DayOfMonth = dayOfMonth,
            IsActive = true
        });
    }

    public async Task<List<PendingIncomeConfirmationDto>> GetPendingConfirmationsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var period = new DateTime(today.Year, today.Month, 1);

        var patterns = await _recurringIncomeRepository.GetAllAsync();
        var confirmations = await _recurringIncomeRepository.GetConfirmationsForPeriodAsync(period);
        var confirmedIds = confirmations.Select(c => c.RecurringIncomeId).ToHashSet();

        return patterns
            .Where(p => p.IsActive && p.DayOfMonth <= today.Day && !confirmedIds.Contains(p.Id))
            .Select(p => new PendingIncomeConfirmationDto(p.Id, p.Source, p.ExpectedAmount, new DateTime(today.Year, today.Month, p.DayOfMonth)))
            .ToList();
    }

    public async Task<RegisterIncomeResult> ConfirmAsync(Guid recurringIncomeId, Guid? accountId, decimal actualAmount)
    {
        var today = DateTime.UtcNow.Date;
        var period = new DateTime(today.Year, today.Month, 1);

        var confirmations = await _recurringIncomeRepository.GetConfirmationsForPeriodAsync(period);
        if (confirmations.Any(c => c.RecurringIncomeId == recurringIncomeId))
        {
            // ya se confirmo este periodo: no volver a sumar el dinero.
            var accountsAlready = await _accountRepository.GetAllAsync();
            return new RegisterIncomeResult(false, "Este ingreso ya fue confirmado este periodo.", accountsAlready.Sum(a => a.Balance), 0);
        }

        if (actualAmount <= 0)
        {
            // no llego nada: se registra la confirmacion, pero no entra dinero.
            await _recurringIncomeRepository.AddConfirmationAsync(new RecurringIncomeConfirmation
            {
                RecurringIncomeId = recurringIncomeId,
                Period = period,
                ConfirmedAmount = 0,
                IncomeId = null,
                ConfirmedAt = DateTime.UtcNow
            });

            var accounts = await _accountRepository.GetAllAsync();
            var total = accounts.Sum(a => a.Balance);
            return new RegisterIncomeResult(true, null, total, total);
        }

        if (accountId is null)
        {
            var accounts = await _accountRepository.GetAllAsync();
            return new RegisterIncomeResult(false, "Elegí a qué cuenta entró el dinero.", accounts.Sum(a => a.Balance), 0);
        }

        var patterns = await _recurringIncomeRepository.GetAllAsync();
        var pattern = patterns.FirstOrDefault(p => p.Id == recurringIncomeId);
        var source = pattern?.Source ?? "Ingreso recurrente";

        var result = await _incomeService.RegisterIncomeAsync(accountId.Value, source, actualAmount);
        if (!result.Success)
        {
            return result;
        }

        await _recurringIncomeRepository.AddConfirmationAsync(new RecurringIncomeConfirmation
        {
            RecurringIncomeId = recurringIncomeId,
            Period = period,
            ConfirmedAmount = actualAmount,
            IncomeId = null,
            ConfirmedAt = DateTime.UtcNow
        });

        return result;
    }

    private static RecurringIncomeDto ToDto(RecurringIncome r) => new(r.Id, r.Source, r.ExpectedAmount, r.DayOfMonth, r.IsActive);
}
