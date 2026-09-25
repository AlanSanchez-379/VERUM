using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseCostRepository : ICostRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseCostRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Cost>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<CostRow>()
            .Where(c => c.UserId == userId)
            .Where(c => c.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Cost cost)
    {
        var row = new CostRow
        {
            UserId = _currentUser.UserId,
            BusinessId = cost.BusinessId,
            Name = cost.Name,
            Amount = cost.Amount,
            DueDate = cost.DueDate,
            IsPaid = cost.IsPaid
        };

        var response = await _client.From<CostRow>().Insert(row);
        cost.Id = response.Models.First().Id;
    }

    public async Task MarkPaidAsync(Guid businessId, Guid id)
    {
        var userId = _currentUser.UserId;
        await _client.From<CostRow>()
            .Where(c => c.UserId == userId)
            .Where(c => c.BusinessId == businessId)
            .Where(c => c.Id == id)
            .Set(c => c.IsPaid, true)
            .Update();
    }

    private static Cost ToEntity(CostRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        Name = row.Name,
        Amount = row.Amount,
        DueDate = row.DueDate,
        IsPaid = row.IsPaid
    };
}
