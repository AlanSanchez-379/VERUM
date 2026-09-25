using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface ICostRepository
{
    Task<List<Cost>> GetAllAsync(Guid businessId);
    Task AddAsync(Cost cost);
    Task MarkPaidAsync(Guid businessId, Guid id);
}
