namespace Verum.Application.DTOs.Negocio;

public record PayableDto(Guid Id, Guid BusinessId, string SupplierName, decimal Amount, DateTime DueDate, bool IsPaid);
