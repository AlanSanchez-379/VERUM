namespace Verum.Application.DTOs.Negocio;

public record BusinessExpenseDto(Guid Id, Guid BusinessId, string Category, decimal Amount, DateTime Date);
