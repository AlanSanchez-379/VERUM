using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Domain.Enums;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseGoalRepository cuando existan credenciales reales.
public class DummyGoalRepository : IGoalRepository
{
    private static readonly object Lock = new();

    private static readonly List<Goal> Goals = new()
    {
        new Goal
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
            Name = "Moto",
            TargetAmount = 65000m,
            CurrentAmount = 40300m,
            TargetDate = new DateTime(2027, 6, 30),
            Priority = GoalPriority.Alta
        },
        new Goal
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
            Name = "Fondo de emergencia",
            TargetAmount = 30000m,
            CurrentAmount = 14400m,
            TargetDate = new DateTime(2027, 12, 31),
            Priority = GoalPriority.Media
        },
        new Goal
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
            Name = "Viaje",
            TargetAmount = 18000m,
            CurrentAmount = 3200m,
            TargetDate = new DateTime(2027, 3, 1),
            Priority = GoalPriority.Baja
        }
    };

    public Task<List<Goal>> GetAllAsync()
    {
        lock (Lock)
        {
            return Task.FromResult(Goals.OrderByDescending(g => g.Priority).ToList());
        }
    }

    public Task<Goal?> GetPriorityGoalAsync()
    {
        lock (Lock)
        {
            return Task.FromResult(Goals.OrderByDescending(g => g.Priority).FirstOrDefault());
        }
    }

    public Task UpdatePriorityAsync(Guid id, GoalPriority priority)
    {
        lock (Lock)
        {
            var goal = Goals.FirstOrDefault(g => g.Id == id);
            if (goal is not null)
            {
                goal.Priority = priority;
            }
        }
        return Task.CompletedTask;
    }
}
