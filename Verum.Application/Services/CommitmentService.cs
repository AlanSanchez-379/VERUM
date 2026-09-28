using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;

namespace Verum.Application.Services;

public class CommitmentService : ICommitmentService
{
    private readonly ICommitmentRepository _commitmentRepository;
    private readonly IExpenseService _expenseService;

    public CommitmentService(ICommitmentRepository commitmentRepository, IExpenseService expenseService)
    {
        _commitmentRepository = commitmentRepository;
        _expenseService = expenseService;
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

    public async Task<CommitmentPayResult> MarkPaidAsync(Guid id, Guid accountId)
    {
        var commitments = await _commitmentRepository.GetAllForCurrentPeriodAsync();
        var commitment = commitments.FirstOrDefault(c => c.Id == id);
        if (commitment is null || commitment.IsPaid)
        {
            return new CommitmentPayResult(false, "El compromiso no existe o ya está pagado.");
        }

        var result = await _expenseService.RegisterExpenseAsync(accountId, $"Compromiso: {commitment.Name}", commitment.Amount);
        if (!result.Success)
        {
            return new CommitmentPayResult(false, result.Error);
        }

        await _commitmentRepository.MarkPaidAsync(id);
        return new CommitmentPayResult(true, null);
    }
}
