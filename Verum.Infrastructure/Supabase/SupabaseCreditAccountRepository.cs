using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseCreditAccountRepository : ICreditAccountRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseCreditAccountRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<CreditAccount>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<CreditAccountRow>().Where(c => c.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    private static CreditAccount ToEntity(CreditAccountRow row) => new()
    {
        Id = row.Id,
        Name = row.Name,
        CreditLimit = row.CreditLimit,
        UsedAmount = row.UsedAmount,
        CutoffDate = row.CutoffDate,
        DueDate = row.DueDate
    };
}
