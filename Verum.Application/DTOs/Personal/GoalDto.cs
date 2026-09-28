namespace Verum.Application.DTOs.Personal;

public record GoalDto(
    Guid Id,
    string Name,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime TargetDate,
    string Priority,
    string? ImageUrl = null)
{
    public decimal Remaining => Math.Max(0, TargetAmount - CurrentAmount);
    public int PercentComplete => TargetAmount <= 0
        ? 0
        : (int)Math.Round(Math.Min(100m, CurrentAmount / TargetAmount * 100m));
}

public record GoalImageResult(bool Success, string? Error, string? ImageUrl);
