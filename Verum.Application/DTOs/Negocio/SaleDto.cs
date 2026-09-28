namespace Verum.Application.DTOs.Negocio;

public record SaleDto(Guid Id, Guid BusinessId, Guid AccountId, string Description, decimal Amount, DateTime Date);
