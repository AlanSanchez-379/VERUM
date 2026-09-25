namespace Verum.Application.DTOs.Negocio;

public record ReceivableDto(Guid Id, Guid BusinessId, string ClientName, decimal Amount, DateTime DueDate, bool IsCollected);
