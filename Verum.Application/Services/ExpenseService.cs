using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IAccountRepository _accountRepository;

    public ExpenseService(IExpenseRepository expenseRepository, IAccountRepository accountRepository)
    {
        _expenseRepository = expenseRepository;
        _accountRepository = accountRepository;
    }

    public async Task<List<ExpenseDto>> GetRecentAsync(int count)
    {
        var expenses = await _expenseRepository.GetRecentAsync(count);
        return expenses
            .Select(e => new ExpenseDto(e.Id, e.AccountId, e.Category, e.Amount, e.Date))
            .ToList();
    }

    public async Task<List<ExpenseDto>> GetAllAsync()
    {
        var expenses = await _expenseRepository.GetAllAsync();
        return expenses
            .Select(e => new ExpenseDto(e.Id, e.AccountId, e.Category, e.Amount, e.Date))
            .ToList();
    }

    public async Task<decimal> GetTotalAsync()
    {
        var expenses = await _expenseRepository.GetAllAsync();
        return expenses.Sum(e => e.Amount);
    }

    public async Task<RegisterExpenseResult> RegisterExpenseAsync(Guid accountId, string category, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return new RegisterExpenseResult(false, "Elegí una categoría.", 0, 0);
        }

        if (amount <= 0)
        {
            return new RegisterExpenseResult(false, "El monto debe ser mayor a cero.", 0, 0);
        }

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
        {
            return new RegisterExpenseResult(false, "La cuenta no existe.", 0, 0);
        }

        var newBalance = account.Balance - amount;
        await _accountRepository.UpdateBalanceAsync(accountId, newBalance);

        await _expenseRepository.AddAsync(new Expense
        {
            AccountId = accountId,
            Category = category.Trim(),
            Amount = amount,
            Date = DateTime.UtcNow
        });

        var accounts = await _accountRepository.GetAllAsync();
        var totalAvailable = accounts.Sum(a => a.Balance);

        return new RegisterExpenseResult(true, null, totalAvailable, newBalance);
    }
}
