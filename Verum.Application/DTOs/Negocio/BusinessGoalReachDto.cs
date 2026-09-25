namespace Verum.Application.DTOs.Negocio;

public record BusinessGoalReachDto(
    BusinessGoalDto Goal,
    string Status,
    decimal RequiredPerMonth,
    int MonthsRemaining);
