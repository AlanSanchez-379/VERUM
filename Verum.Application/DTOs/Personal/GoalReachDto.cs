namespace Verum.Application.DTOs.Personal;

public record GoalReachDto(
    GoalDto Goal,
    string Status,
    decimal RequiredPerMonth,
    int MonthsRemaining);
