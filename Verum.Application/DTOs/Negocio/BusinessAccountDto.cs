namespace Verum.Application.DTOs.Negocio;

public record BusinessAccountDto(Guid Id, Guid BusinessId, string Name, string Subtitle, decimal Balance);
