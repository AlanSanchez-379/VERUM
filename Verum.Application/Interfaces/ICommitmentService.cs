using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface ICommitmentService
{
    Task<List<CommitmentDto>> GetCurrentPeriodAsync();
    Task<decimal> GetTotalAsync();
    Task<CommitmentPayResult> MarkPaidAsync(Guid id, Guid accountId);
}
