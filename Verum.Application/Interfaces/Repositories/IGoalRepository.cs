using Verum.Domain.Entities;
using Verum.Domain.Enums;

namespace Verum.Application.Interfaces.Repositories;

public interface IGoalRepository
{
    Task<List<Goal>> GetAllAsync();
    Task<Goal?> GetPriorityGoalAsync();
    Task UpdatePriorityAsync(Guid id, GoalPriority priority);
}
