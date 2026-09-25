using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IAccountRepository
{
    Task<List<Account>> GetAllAsync();
    Task<Account?> GetByIdAsync(Guid id);
    Task UpdateBalanceAsync(Guid id, decimal newBalance);
}
