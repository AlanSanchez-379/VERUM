using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IGoalService
{
    Task<List<GoalDto>> GetAllAsync();
    Task<GoalDto?> GetPriorityGoalAsync();
    Task<List<GoalReachDto>> GetReachAnalysisAsync(decimal currentMargin);
    Task UpdatePriorityAsync(Guid id, string priority);
}
