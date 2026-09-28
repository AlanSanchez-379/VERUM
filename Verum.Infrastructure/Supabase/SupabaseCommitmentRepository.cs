using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseCommitmentRepository : ICommitmentRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseCommitmentRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Commitment>> GetAllForCurrentPeriodAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<CommitmentRow>().Where(c => c.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task MarkPaidAsync(Guid id)
    {
        var userId = _currentUser.UserId;
        await _client.From<CommitmentRow>()
            .Where(c => c.UserId == userId)
            .Where(c => c.Id == id)
            .Set(c => c.IsPaid, true)
            .Update();
    }

    private static Commitment ToEntity(CommitmentRow row) => new()
    {
        Id = row.Id,
        Name = row.Name,
        Amount = row.Amount,
        DueDate = row.DueDate,
        IsPaid = row.IsPaid
    };
}
