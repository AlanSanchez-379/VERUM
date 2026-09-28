using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseBusinessAccountRepository : IBusinessAccountRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseBusinessAccountRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<BusinessAccount>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessAccountRow>()
            .Where(a => a.UserId == userId)
            .Where(a => a.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<BusinessAccount?> GetByIdAsync(Guid businessId, Guid id)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessAccountRow>()
            .Where(a => a.UserId == userId)
            .Where(a => a.BusinessId == businessId)
            .Where(a => a.Id == id)
            .Get();
        var row = response.Models.FirstOrDefault();
        return row is null ? null : ToEntity(row);
    }

    public async Task<BusinessAccount> CreateAsync(BusinessAccount account)
    {
        var row = new BusinessAccountRow
        {
            UserId = _currentUser.UserId,
            BusinessId = account.BusinessId,
            Name = account.Name,
            Subtitle = account.Subtitle,
            Balance = account.Balance
        };

        var response = await _client.From<BusinessAccountRow>().Insert(row);
        return ToEntity(response.Models.First());
    }

    public async Task UpdateBalanceAsync(Guid businessId, Guid id, decimal newBalance)
    {
        var userId = _currentUser.UserId;
        await _client.From<BusinessAccountRow>()
            .Where(a => a.UserId == userId)
            .Where(a => a.BusinessId == businessId)
            .Where(a => a.Id == id)
            .Set(a => a.Balance, newBalance)
            .Update();
    }

    private static BusinessAccount ToEntity(BusinessAccountRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        Name = row.Name,
        Subtitle = row.Subtitle,
        Balance = row.Balance
    };
}
