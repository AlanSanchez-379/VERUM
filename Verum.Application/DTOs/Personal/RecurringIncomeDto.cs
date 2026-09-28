namespace Verum.Application.DTOs.Personal;

public record RecurringIncomeDto(Guid Id, string Source, decimal ExpectedAmount, int DayOfMonth, bool IsActive);

public record PendingIncomeConfirmationDto(Guid RecurringIncomeId, string Source, decimal ExpectedAmount, DateTime ExpectedDate);
