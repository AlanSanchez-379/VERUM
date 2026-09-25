namespace Verum.Application.DTOs.Negocio;

public record TaxDto(Guid Id, Guid BusinessId, string Name, decimal Amount, DateTime DueDate, bool IsPaid);
