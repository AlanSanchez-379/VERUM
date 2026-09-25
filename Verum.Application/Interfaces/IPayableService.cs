using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface IPayableService
{
    Task<List<PayableDto>> GetAllAsync(Guid businessId);
    Task RegisterAsync(Guid businessId, string supplierName, decimal amount, DateTime dueDate);
    Task MarkPaidAsync(Guid businessId, Guid id);
}
