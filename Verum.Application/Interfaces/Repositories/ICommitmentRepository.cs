using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface ICommitmentRepository
{
    Task<List<Commitment>> GetAllForCurrentPeriodAsync();
    Task MarkPaidAsync(Guid id);
}
