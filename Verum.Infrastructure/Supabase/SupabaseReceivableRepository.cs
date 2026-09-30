using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseReceivableRepository : IReceivableRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseReceivableRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Receivable>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<ReceivableRow>()
            .Where(r => r.UserId == userId)
            .Where(r => r.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Receivable receivable)
    {
        var row = new ReceivableRow
        {
            UserId = _currentUser.UserId,
            BusinessId = receivable.BusinessId,
            ClientName = receivable.ClientName,
            Description = receivable.Description,
            Amount = receivable.Amount,
            DueDate = receivable.DueDate,
            IsCollected = receivable.IsCollected
        };

        var response = await _client.From<ReceivableRow>().Insert(row);
        receivable.Id = response.Models.First().Id;
    }

    public async Task MarkCollectedAsync(Guid businessId, Guid id)
    {
        var userId = _currentUser.UserId;
        await _client.From<ReceivableRow>()
            .Where(r => r.UserId == userId)
            .Where(r => r.BusinessId == businessId)
            .Where(r => r.Id == id)
            .Set(r => r.IsCollected, true)
            .Update();
    }

    private static Receivable ToEntity(ReceivableRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        ClientName = row.ClientName,
        Description = row.Description,
        Amount = row.Amount,
        DueDate = row.DueDate,
        IsCollected = row.IsCollected
    };
}
