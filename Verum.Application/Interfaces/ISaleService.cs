using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface ISaleService
{
    Task<List<SaleDto>> GetRecentAsync(Guid businessId, int count);
    Task<List<SaleDto>> GetAllAsync(Guid businessId);
    Task<decimal> GetTotalAsync(Guid businessId);
    Task RegisterSaleAsync(Guid businessId, Guid accountId, string description, decimal amount);
}
