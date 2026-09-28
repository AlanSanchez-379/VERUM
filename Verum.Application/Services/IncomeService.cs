using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _incomeRepository;
    private readonly IAccountRepository _accountRepository;

    public IncomeService(IIncomeRepository incomeRepository, IAccountRepository accountRepository)
    {
        _incomeRepository = incomeRepository;
        _accountRepository = accountRepository;
    }

    public async Task<List<IncomeDto>> GetCurrentPeriodAsync()
    {
        var incomes = await _incomeRepository.GetAllForCurrentPeriodAsync();
        return incomes
            .Select(i => new IncomeDto(i.Id, i.AccountId, i.Source, i.Amount, i.ExpectedDate, i.IsReceived))
            .ToList();
    }

    public async Task<decimal> GetTotalReceivedAsync()
    {
        var incomes = await _incomeRepository.GetAllForCurrentPeriodAsync();
        return incomes.Where(i => i.IsReceived).Sum(i => i.Amount);
    }

    public async Task<RegisterIncomeResult> RegisterIncomeAsync(Guid accountId, string source, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return new RegisterIncomeResult(false, "Elegí un origen.", 0, 0);
        }

        if (amount <= 0)
        {
            return new RegisterIncomeResult(false, "El monto debe ser mayor a cero.", 0, 0);
        }

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
        {
            return new RegisterIncomeResult(false, "La cuenta no existe.", 0, 0);
        }

        var newBalance = account.Balance + amount;
        await _accountRepository.UpdateBalanceAsync(accountId, newBalance);

        await _incomeRepository.AddAsync(new Income
        {
            AccountId = accountId,
            Source = source.Trim(),
            Amount = amount,
            ExpectedDate = DateTime.UtcNow,
            IsReceived = true
        });

        var accounts = await _accountRepository.GetAllAsync();
        var totalAvailable = accounts.Sum(a => a.Balance);

        return new RegisterIncomeResult(true, null, totalAvailable, newBalance);
    }
}
