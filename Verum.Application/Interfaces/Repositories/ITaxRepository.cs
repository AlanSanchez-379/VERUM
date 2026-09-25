using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface ITaxRepository
{
    Task<List<Tax>> GetAllAsync(Guid businessId);
    Task AddAsync(Tax tax);
    Task MarkPaidAsync(Guid businessId, Guid id);
}
