using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface ICreditAccountRepository
{
    Task<List<CreditAccount>> GetAllAsync();
}
