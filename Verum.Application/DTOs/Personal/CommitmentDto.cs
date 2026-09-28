namespace Verum.Application.DTOs.Personal;

public record CommitmentDto(Guid Id, string Name, decimal Amount, DateTime DueDate, bool IsPaid);

public record CommitmentPayResult(bool Success, string? Error);
