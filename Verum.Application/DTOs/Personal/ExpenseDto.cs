namespace Verum.Application.DTOs.Personal;

public record ExpenseDto(Guid Id, Guid AccountId, string Category, decimal Amount, DateTime Date);

public record RegisterExpenseResult(
    bool Success,
    string? Error,
    decimal TotalAvailable,
    decimal AccountBalance);
