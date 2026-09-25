using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IReceivableRepository
{
    Task<List<Receivable>> GetAllAsync(Guid businessId);
    Task AddAsync(Receivable receivable);
    Task MarkCollectedAsync(Guid businessId, Guid id);
}
