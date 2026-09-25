using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface ITaxService
{
    Task<List<TaxDto>> GetAllAsync(Guid businessId);
    Task RegisterAsync(Guid businessId, string name, decimal amount, DateTime dueDate);
    Task MarkPaidAsync(Guid businessId, Guid id);
}
