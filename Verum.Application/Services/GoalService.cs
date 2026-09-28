using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Enums;
using Verum.Domain.Rules;

namespace Verum.Application.Services;

public class GoalService : IGoalService
{
    private const long MaxImageBytes = 3 * 1024 * 1024; // 3MB tope tras compresión en el cliente

    private readonly IGoalRepository _goalRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IGoalImageStorage _imageStorage;

    public GoalService(IGoalRepository goalRepository, IAccountRepository accountRepository, IGoalImageStorage imageStorage)
    {
        _goalRepository = goalRepository;
        _accountRepository = accountRepository;
        _imageStorage = imageStorage;
    }

    public async Task<List<GoalDto>> GetAllAsync()
    {
        var goals = await _goalRepository.GetAllAsync();
        var totalAvailable = await GetTotalAvailableAsync();
        var dtos = new List<GoalDto>();
        foreach (var g in goals)
        {
            dtos.Add(await ToDtoAsync(g, totalAvailable));
        }
        return dtos;
    }

    public async Task<GoalDto?> GetPriorityGoalAsync()
    {
        var goal = await _goalRepository.GetPriorityGoalAsync();
        if (goal is null)
        {
            return null;
        }

        var totalAvailable = await GetTotalAvailableAsync();
        return await ToDtoAsync(goal, totalAvailable);
    }

    public async Task<List<GoalReachDto>> GetReachAnalysisAsync(decimal currentMargin)
    {
        var goals = await _goalRepository.GetAllAsync();
        var totalAvailable = await GetTotalAvailableAsync();
        var today = DateTime.UtcNow.Date;

        var result = new List<GoalReachDto>();
        foreach (var g in goals)
        {
            var dto = await ToDtoAsync(g, totalAvailable);
            var monthsRemaining = Math.Max(0, ((g.TargetDate.Year - today.Year) * 12) + g.TargetDate.Month - today.Month);
            var required = ProjectionRule.RequiredPerPeriod(dto.Remaining, monthsRemaining);
            var status = ProjectionRule.ClassifyReach(dto.Remaining, monthsRemaining, currentMargin);

            result.Add(new GoalReachDto(dto, StatusLabel(status), required, monthsRemaining));
        }
        return result;
    }

    private async Task<decimal> GetTotalAvailableAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Sum(a => a.Balance);
    }

    public async Task UpdatePriorityAsync(Guid id, string priority)
    {
        if (Enum.TryParse<GoalPriority>(priority, ignoreCase: true, out var parsed))
        {
            await _goalRepository.UpdatePriorityAsync(id, parsed);
        }
    }

    public async Task<GoalImageResult> SetImageAsync(Guid id, byte[] bytes, string contentType)
    {
        if (bytes.Length == 0)
        {
            return new GoalImageResult(false, "La imagen está vacía.", null);
        }

        if (bytes.Length > MaxImageBytes)
        {
            return new GoalImageResult(false, "La imagen sigue siendo muy pesada. Probá con otra.", null);
        }

        if (contentType != "image/jpeg" && contentType != "image/png" && contentType != "image/webp")
        {
            return new GoalImageResult(false, "Formato de imagen no soportado.", null);
        }

        var goal = await _goalRepository.GetByIdAsync(id);
        if (goal is null)
        {
            return new GoalImageResult(false, "La meta no existe.", null);
        }

        var path = await _imageStorage.UploadAsync(id, bytes, contentType);
        await _goalRepository.UpdateImageAsync(id, path);
        var url = await _imageStorage.GetSignedUrlAsync(path);
        return new GoalImageResult(true, null, url);
    }

    public async Task RemoveImageAsync(Guid id)
    {
        var goal = await _goalRepository.GetByIdAsync(id);
        if (goal is null || string.IsNullOrEmpty(goal.ImagePath))
        {
            return;
        }

        await _imageStorage.DeleteAsync(goal.ImagePath);
        await _goalRepository.UpdateImageAsync(id, null);
    }

    private static string StatusLabel(Domain.Rules.ReachStatus status) => status switch
    {
        Domain.Rules.ReachStatus.DentroDeAlcance => "Dentro de alcance",
        Domain.Rules.ReachStatus.CercaDelLimite => "Cerca del límite",
        _ => "Fuera de alcance"
    };

    private async Task<GoalDto> ToDtoAsync(Domain.Entities.Goal g, decimal totalAvailable)
    {
        var imageUrl = string.IsNullOrEmpty(g.ImagePath) ? null : await _imageStorage.GetSignedUrlAsync(g.ImagePath);
        return new GoalDto(g.Id, g.Name, g.TargetAmount, totalAvailable, g.TargetDate, g.Priority.ToString(), imageUrl);
    }
}
