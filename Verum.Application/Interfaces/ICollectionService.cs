using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface ICollectionService
{
    Task<List<ReceivableDto>> GetAllAsync(Guid businessId);
    Task RegisterAsync(Guid businessId, string clientName, decimal amount, DateTime dueDate);
    Task MarkCollectedAsync(Guid businessId, Guid id);
}
