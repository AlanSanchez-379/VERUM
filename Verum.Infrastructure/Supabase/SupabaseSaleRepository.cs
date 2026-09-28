using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseSaleRepository : ISaleRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseSaleRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Sale>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<SaleRow>()
            .Where(s => s.UserId == userId)
            .Where(s => s.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<List<Sale>> GetRecentAsync(Guid businessId, int count)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<SaleRow>()
            .Where(s => s.UserId == userId)
            .Where(s => s.BusinessId == businessId)
            .Order(s => s.Date, Postgrest.Constants.Ordering.Descending)
            .Limit(count)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Sale sale)
    {
        var row = new SaleRow
        {
            UserId = _currentUser.UserId,
            BusinessId = sale.BusinessId,
            AccountId = sale.AccountId,
            Description = sale.Description,
            Amount = sale.Amount,
            Date = sale.Date
        };

        var response = await _client.From<SaleRow>().Insert(row);
        sale.Id = response.Models.First().Id;
    }

    private static Sale ToEntity(SaleRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        AccountId = row.AccountId,
        Description = row.Description,
        Amount = row.Amount,
        Date = row.Date
    };
}
