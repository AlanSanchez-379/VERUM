using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IExpenseService
{
    Task<List<ExpenseDto>> GetRecentAsync(int count);
    Task<List<ExpenseDto>> GetAllAsync();
    Task<decimal> GetTotalAsync();
    Task<RegisterExpenseResult> RegisterExpenseAsync(Guid accountId, string category, decimal amount);
}
