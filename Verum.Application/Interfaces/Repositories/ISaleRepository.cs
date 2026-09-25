using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface ISaleRepository
{
    Task<List<Sale>> GetAllAsync(Guid businessId);
    Task<List<Sale>> GetRecentAsync(Guid businessId, int count);
    Task AddAsync(Sale sale);
}
