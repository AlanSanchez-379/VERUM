using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class BusinessExpenseService : IBusinessExpenseService
{
    private readonly IBusinessExpenseRepository _expenseRepository;
    private readonly IBusinessAccountRepository _accountRepository;

    public BusinessExpenseService(IBusinessExpenseRepository expenseRepository, IBusinessAccountRepository accountRepository)
    {
        _expenseRepository = expenseRepository;
        _accountRepository = accountRepository;
    }

    public async Task<List<BusinessExpenseDto>> GetRecentAsync(Guid businessId, int count)
    {
        var expenses = await _expenseRepository.GetRecentAsync(businessId, count);
        return expenses.Select(ToDto).ToList();
    }

    public async Task<List<BusinessExpenseDto>> GetAllAsync(Guid businessId)
    {
        var expenses = await _expenseRepository.GetAllAsync(businessId);
        return expenses.Select(ToDto).ToList();
    }

    public async Task<decimal> GetTotalAsync(Guid businessId)
    {
        var expenses = await _expenseRepository.GetAllAsync(businessId);
        return expenses.Sum(e => e.Amount);
    }

    public async Task<BusinessExpenseResult> RegisterExpenseAsync(Guid businessId, Guid accountId, string category, decimal amount)
    {
        if (amount <= 0)
        {
            return new BusinessExpenseResult(false, "El monto debe ser mayor a cero.");
        }

        var account = await _accountRepository.GetByIdAsync(businessId, accountId);
        if (account is null)
        {
            return new BusinessExpenseResult(false, "La cuenta no existe.");
        }

        if (account.Balance < amount)
        {
            return new BusinessExpenseResult(false, $"No hay suficiente saldo en {account.Name} para este gasto.");
        }

        await _accountRepository.UpdateBalanceAsync(businessId, accountId, account.Balance - amount);

        await _expenseRepository.AddAsync(new BusinessExpense
        {
            BusinessId = businessId,
            AccountId = accountId,
            Category = category,
            Amount = amount,
            Date = DateTime.UtcNow
        });

        return new BusinessExpenseResult(true, null);
    }

    private static BusinessExpenseDto ToDto(BusinessExpense e) => new(e.Id, e.BusinessId, e.AccountId, e.Category, e.Amount, e.Date);
}
