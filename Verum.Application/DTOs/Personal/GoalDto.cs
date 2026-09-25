namespace Verum.Application.DTOs.Personal;

public record GoalDto(
    Guid Id,
    string Name,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateTime TargetDate,
    string Priority)
{
    public decimal Remaining => Math.Max(0, TargetAmount - CurrentAmount);
    public int PercentComplete => TargetAmount <= 0
        ? 0
        : (int)Math.Round(Math.Min(100m, CurrentAmount / TargetAmount * 100m));
}
