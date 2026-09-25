namespace Verum.Application.DTOs.Negocio;

public record SaleDto(Guid Id, Guid BusinessId, string Description, decimal Amount, DateTime Date);
