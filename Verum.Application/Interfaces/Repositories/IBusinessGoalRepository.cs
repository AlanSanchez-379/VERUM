using Verum.Domain.Entities;
using Verum.Domain.Enums;

namespace Verum.Application.Interfaces.Repositories;

public interface IBusinessGoalRepository
{
    Task<List<BusinessGoal>> GetAllAsync(Guid businessId);
    Task AddAsync(BusinessGoal goal);
    Task UpdatePriorityAsync(Guid businessId, Guid id, GoalPriority priority);
    Task AddContributionAsync(Guid businessId, Guid id, decimal amount);
}
