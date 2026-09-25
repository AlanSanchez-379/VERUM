using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface IInvestmentService
{
    Task<List<InvestmentDto>> GetAllAsync(Guid businessId);
    Task<decimal> GetTotalAsync(Guid businessId);
    Task RegisterAsync(Guid businessId, string description, decimal amount);
}
