using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseTransferRepository : ITransferRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseTransferRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Transfer>> GetRecentAsync(Guid businessId, int take)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<TransferRow>()
            .Where(t => t.UserId == userId)
            .Where(t => t.BusinessId == businessId)
            .Order(t => t.Date, Postgrest.Constants.Ordering.Descending)
            .Limit(take)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Transfer transfer)
    {
        var row = new TransferRow
        {
            UserId = _currentUser.UserId,
            BusinessId = transfer.BusinessId,
            PersonalAccountId = transfer.PersonalAccountId,
            BusinessAccountId = transfer.BusinessAccountId,
            Direction = transfer.Direction == TransferDirection.ToBusiness ? "to_business" : "to_personal",
            Amount = transfer.Amount,
            Note = transfer.Note,
            Date = transfer.Date
        };

        await _client.From<TransferRow>().Insert(row);
    }

    private static Transfer ToEntity(TransferRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        PersonalAccountId = row.PersonalAccountId,
        BusinessAccountId = row.BusinessAccountId,
        Direction = row.Direction == "to_business" ? TransferDirection.ToBusiness : TransferDirection.ToPersonal,
        Amount = row.Amount,
        Note = row.Note,
        Date = row.Date
    };
}
