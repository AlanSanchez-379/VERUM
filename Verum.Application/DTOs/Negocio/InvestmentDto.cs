namespace Verum.Application.DTOs.Negocio;

public record InvestmentDto(Guid Id, Guid BusinessId, string Description, decimal Amount, DateTime Date);
