namespace Verum.Application.DTOs.Personal;

public record IncomeDto(Guid Id, string Source, decimal Amount, DateTime ExpectedDate, bool IsReceived);
