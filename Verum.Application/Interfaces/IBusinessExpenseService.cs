using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface IBusinessExpenseService
{
    Task<List<BusinessExpenseDto>> GetRecentAsync(Guid businessId, int count);
    Task<List<BusinessExpenseDto>> GetAllAsync(Guid businessId);
    Task<decimal> GetTotalAsync(Guid businessId);
    Task RegisterExpenseAsync(Guid businessId, Guid accountId, string category, decimal amount);
}
