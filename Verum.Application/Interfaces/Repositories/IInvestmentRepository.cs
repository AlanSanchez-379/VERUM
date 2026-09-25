using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IInvestmentRepository
{
    Task<List<Investment>> GetAllAsync(Guid businessId);
    Task AddAsync(Investment investment);
}
