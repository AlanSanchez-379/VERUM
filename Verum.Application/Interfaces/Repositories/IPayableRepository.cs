using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IPayableRepository
{
    Task<List<Payable>> GetAllAsync(Guid businessId);
    Task AddAsync(Payable payable);
    Task MarkPaidAsync(Guid businessId, Guid id);
}
