using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseTaxRepository : ITaxRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseTaxRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Tax>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<TaxRow>()
            .Where(t => t.UserId == userId)
            .Where(t => t.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Tax tax)
    {
        var row = new TaxRow
        {
            UserId = _currentUser.UserId,
            BusinessId = tax.BusinessId,
            Name = tax.Name,
            Amount = tax.Amount,
            DueDate = tax.DueDate,
            IsPaid = tax.IsPaid
        };

        var response = await _client.From<TaxRow>().Insert(row);
        tax.Id = response.Models.First().Id;
    }

    public async Task MarkPaidAsync(Guid businessId, Guid id)
    {
        var userId = _currentUser.UserId;
        await _client.From<TaxRow>()
            .Where(t => t.UserId == userId)
            .Where(t => t.BusinessId == businessId)
            .Where(t => t.Id == id)
            .Set(t => t.IsPaid, true)
            .Update();
    }

    private static Tax ToEntity(TaxRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        Name = row.Name,
        Amount = row.Amount,
        DueDate = row.DueDate,
        IsPaid = row.IsPaid
    };
}
