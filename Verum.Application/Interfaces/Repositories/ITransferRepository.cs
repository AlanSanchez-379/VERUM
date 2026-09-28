using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface ITransferRepository
{
    Task<List<Transfer>> GetRecentAsync(Guid businessId, int take);
    Task AddAsync(Transfer transfer);
}
