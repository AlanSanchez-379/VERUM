using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IBusinessAccountRepository
{
    Task<List<BusinessAccount>> GetAllAsync(Guid businessId);
    Task<BusinessAccount?> GetByIdAsync(Guid businessId, Guid id);
    Task<BusinessAccount> CreateAsync(BusinessAccount account);
    Task UpdateBalanceAsync(Guid businessId, Guid id, decimal newBalance);
}
