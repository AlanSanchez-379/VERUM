using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabasePayableRepository : IPayableRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabasePayableRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Payable>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<PayableRow>()
            .Where(p => p.UserId == userId)
            .Where(p => p.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Payable payable)
    {
        var row = new PayableRow
        {
            UserId = _currentUser.UserId,
            BusinessId = payable.BusinessId,
            SupplierName = payable.SupplierName,
            Amount = payable.Amount,
            DueDate = payable.DueDate,
            IsPaid = payable.IsPaid
        };

        var response = await _client.From<PayableRow>().Insert(row);
        payable.Id = response.Models.First().Id;
    }

    public async Task MarkPaidAsync(Guid businessId, Guid id)
    {
        var userId = _currentUser.UserId;
        await _client.From<PayableRow>()
            .Where(p => p.UserId == userId)
            .Where(p => p.BusinessId == businessId)
            .Where(p => p.Id == id)
            .Set(p => p.IsPaid, true)
            .Update();
    }

    private static Payable ToEntity(PayableRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        SupplierName = row.SupplierName,
        Amount = row.Amount,
        DueDate = row.DueDate,
        IsPaid = row.IsPaid
    };
}
