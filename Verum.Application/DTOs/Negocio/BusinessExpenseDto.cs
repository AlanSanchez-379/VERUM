namespace Verum.Application.DTOs.Negocio;

public record BusinessExpenseDto(Guid Id, Guid BusinessId, Guid AccountId, string Category, decimal Amount, DateTime Date);
