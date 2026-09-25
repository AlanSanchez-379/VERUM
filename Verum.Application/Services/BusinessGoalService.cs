using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Domain.Enums;
using Verum.Domain.Rules;

namespace Verum.Application.Services;

public class BusinessGoalService : IBusinessGoalService
{
    private readonly IBusinessGoalRepository _goalRepository;

    public BusinessGoalService(IBusinessGoalRepository goalRepository)
    {
        _goalRepository = goalRepository;
    }

    public async Task<List<BusinessGoalDto>> GetAllAsync(Guid businessId)
    {
        var goals = await _goalRepository.GetAllAsync(businessId);
        return goals.OrderByDescending(g => g.Priority).Select(ToDto).ToList();
    }

    public async Task RegisterAsync(Guid businessId, string name, decimal targetAmount, DateTime targetDate)
    {
        if (targetAmount <= 0)
        {
            return;
        }

        await _goalRepository.AddAsync(new BusinessGoal
        {
            BusinessId = businessId,
            Name = name,
            TargetAmount = targetAmount,
            CurrentAmount = 0,
            TargetDate = targetDate,
            Priority = GoalPriority.Media
        });
    }

    public async Task<List<BusinessGoalReachDto>> GetReachAnalysisAsync(Guid businessId, decimal currentMargin)
    {
        var goals = await _goalRepository.GetAllAsync(businessId);
        var today = DateTime.UtcNow.Date;

        return goals.Select(g =>
        {
            var dto = ToDto(g);
            var monthsRemaining = Math.Max(0, ((g.TargetDate.Year - today.Year) * 12) + g.TargetDate.Month - today.Month);
            var required = ProjectionRule.RequiredPerPeriod(dto.Remaining, monthsRemaining);
            var status = ProjectionRule.ClassifyReach(dto.Remaining, monthsRemaining, currentMargin);

            return new BusinessGoalReachDto(dto, StatusLabel(status), required, monthsRemaining);
        }).ToList();
    }

    private static string StatusLabel(ReachStatus status) => status switch
    {
        ReachStatus.DentroDeAlcance => "Dentro de alcance",
        ReachStatus.CercaDelLimite => "Cerca del límite",
        _ => "Fuera de alcance"
    };

    public Task UpdatePriorityAsync(Guid businessId, Guid id, string priority)
    {
        if (!Enum.TryParse<GoalPriority>(priority, ignoreCase: true, out var parsed))
        {
            return Task.CompletedTask;
        }

        return _goalRepository.UpdatePriorityAsync(businessId, id, parsed);
    }

    public Task AddContributionAsync(Guid businessId, Guid id, decimal amount)
    {
        if (amount <= 0)
        {
            return Task.CompletedTask;
        }

        return _goalRepository.AddContributionAsync(businessId, id, amount);
    }

    private static BusinessGoalDto ToDto(BusinessGoal g) => new(g.Id, g.BusinessId, g.Name, g.TargetAmount, g.CurrentAmount, g.TargetDate, g.Priority.ToString());
}
