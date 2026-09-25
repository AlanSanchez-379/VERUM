using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Enums;
using Verum.Domain.Rules;

namespace Verum.Application.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository _goalRepository;

    public GoalService(IGoalRepository goalRepository)
    {
        _goalRepository = goalRepository;
    }

    public async Task<List<GoalDto>> GetAllAsync()
    {
        var goals = await _goalRepository.GetAllAsync();
        return goals.Select(ToDto).ToList();
    }

    public async Task<GoalDto?> GetPriorityGoalAsync()
    {
        var goal = await _goalRepository.GetPriorityGoalAsync();
        return goal is null ? null : ToDto(goal);
    }

    public async Task<List<GoalReachDto>> GetReachAnalysisAsync(decimal currentMargin)
    {
        var goals = await _goalRepository.GetAllAsync();
        var today = DateTime.UtcNow.Date;

        return goals.Select(g =>
        {
            var dto = ToDto(g);
            var monthsRemaining = Math.Max(0, ((g.TargetDate.Year - today.Year) * 12) + g.TargetDate.Month - today.Month);
            var required = ProjectionRule.RequiredPerPeriod(dto.Remaining, monthsRemaining);
            var status = ProjectionRule.ClassifyReach(dto.Remaining, monthsRemaining, currentMargin);

            return new GoalReachDto(dto, StatusLabel(status), required, monthsRemaining);
        }).ToList();
    }

    public async Task UpdatePriorityAsync(Guid id, string priority)
    {
        if (Enum.TryParse<GoalPriority>(priority, ignoreCase: true, out var parsed))
        {
            await _goalRepository.UpdatePriorityAsync(id, parsed);
        }
    }

    private static string StatusLabel(Domain.Rules.ReachStatus status) => status switch
    {
        Domain.Rules.ReachStatus.DentroDeAlcance => "Dentro de alcance",
        Domain.Rules.ReachStatus.CercaDelLimite => "Cerca del límite",
        _ => "Fuera de alcance"
    };

    private static GoalDto ToDto(Domain.Entities.Goal g) =>
        new(g.Id, g.Name, g.TargetAmount, g.CurrentAmount, g.TargetDate, g.Priority.ToString());
}
