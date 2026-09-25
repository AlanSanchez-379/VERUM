using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;

namespace Verum.Application.Services;

public class CommitmentService : ICommitmentService
{
    private readonly ICommitmentRepository _commitmentRepository;

    public CommitmentService(ICommitmentRepository commitmentRepository)
    {
        _commitmentRepository = commitmentRepository;
    }

    public async Task<List<CommitmentDto>> GetCurrentPeriodAsync()
    {
        var commitments = await _commitmentRepository.GetAllForCurrentPeriodAsync();
        return commitments
            .Select(c => new CommitmentDto(c.Id, c.Name, c.Amount, c.DueDate, c.IsPaid))
            .ToList();
    }

    public async Task<decimal> GetTotalAsync()
    {
        var commitments = await _commitmentRepository.GetAllForCurrentPeriodAsync();
        return commitments.Sum(c => c.Amount);
    }
}
