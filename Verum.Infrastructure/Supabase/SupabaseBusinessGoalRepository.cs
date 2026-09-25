using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Domain.Enums;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseBusinessGoalRepository : IBusinessGoalRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseBusinessGoalRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<BusinessGoal>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessGoalRow>()
            .Where(g => g.UserId == userId)
            .Where(g => g.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(BusinessGoal goal)
    {
        var row = new BusinessGoalRow
        {
            UserId = _currentUser.UserId,
            BusinessId = goal.BusinessId,
            Name = goal.Name,
            TargetAmount = goal.TargetAmount,
            CurrentAmount = goal.CurrentAmount,
            TargetDate = goal.TargetDate,
            Priority = goal.Priority.ToString()
        };

        var response = await _client.From<BusinessGoalRow>().Insert(row);
        goal.Id = response.Models.First().Id;
    }

    public async Task UpdatePriorityAsync(Guid businessId, Guid id, GoalPriority priority)
    {
        var userId = _currentUser.UserId;
        await _client.From<BusinessGoalRow>()
            .Where(g => g.UserId == userId)
            .Where(g => g.BusinessId == businessId)
            .Where(g => g.Id == id)
            .Set(g => g.Priority, priority.ToString())
            .Update();
    }

    public async Task AddContributionAsync(Guid businessId, Guid id, decimal amount)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessGoalRow>()
            .Where(g => g.UserId == userId)
            .Where(g => g.BusinessId == businessId)
            .Where(g => g.Id == id)
            .Get();

        var current = response.Models.FirstOrDefault();
        if (current is null)
        {
            return;
        }

        await _client.From<BusinessGoalRow>()
            .Where(g => g.UserId == userId)
            .Where(g => g.BusinessId == businessId)
            .Where(g => g.Id == id)
            .Set(g => g.CurrentAmount, current.CurrentAmount + amount)
            .Update();
    }

    private static BusinessGoal ToEntity(BusinessGoalRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        Name = row.Name,
        TargetAmount = row.TargetAmount,
        CurrentAmount = row.CurrentAmount,
        TargetDate = row.TargetDate,
        Priority = Enum.Parse<GoalPriority>(row.Priority)
    };
}
