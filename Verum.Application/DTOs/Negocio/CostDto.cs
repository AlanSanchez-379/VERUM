namespace Verum.Application.DTOs.Negocio;

public record CostDto(Guid Id, Guid BusinessId, string Name, decimal Amount, DateTime DueDate, bool IsPaid);
