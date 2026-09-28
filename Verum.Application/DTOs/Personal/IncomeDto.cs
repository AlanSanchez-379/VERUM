namespace Verum.Application.DTOs.Personal;

public record IncomeDto(Guid Id, Guid AccountId, string Source, decimal Amount, DateTime ExpectedDate, bool IsReceived);

public record RegisterIncomeResult(
    bool Success,
    string? Error,
    decimal TotalAvailable,
    decimal AccountBalance);
