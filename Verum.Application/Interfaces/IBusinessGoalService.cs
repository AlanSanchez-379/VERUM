using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface IBusinessGoalService
{
    Task<List<BusinessGoalDto>> GetAllAsync(Guid businessId);
    Task<List<BusinessGoalReachDto>> GetReachAnalysisAsync(Guid businessId, decimal currentMargin);
    Task RegisterAsync(Guid businessId, string name, decimal targetAmount, DateTime targetDate);
    Task UpdatePriorityAsync(Guid businessId, Guid id, string priority);
    Task AddContributionAsync(Guid businessId, Guid id, decimal amount);
}
