using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IDebtRepository
{
    Task<List<Debt>> GetAllAsync();
}
