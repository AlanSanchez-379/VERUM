namespace Verum.Application.DTOs.Negocio;

public record ReceivableDto(Guid Id, Guid BusinessId, string ClientName, string Description, decimal Amount, DateTime DueDate, bool IsCollected);

public record CollectResult(bool Success, string? Error);
