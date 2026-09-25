using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Domain.Enums;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseGoalRepository : IGoalRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseGoalRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Goal>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<GoalRow>().Where(g => g.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<Goal?> GetPriorityGoalAsync()
    {
        var goals = await GetAllAsync();
        return goals.OrderByDescending(g => g.Priority).FirstOrDefault();
    }

    public async Task UpdatePriorityAsync(Guid id, GoalPriority priority)
    {
        var userId = _currentUser.UserId;
        await _client.From<GoalRow>()
            .Where(g => g.UserId == userId)
            .Where(g => g.Id == id)
            .Set(g => g.Priority, priority.ToString())
            .Update();
    }

    private static Goal ToEntity(GoalRow row) => new()
    {
        Id = row.Id,
        Name = row.Name,
        TargetAmount = row.TargetAmount,
        CurrentAmount = row.CurrentAmount,
        TargetDate = row.TargetDate,
        Priority = Enum.Parse<GoalPriority>(row.Priority)
    };
}
