using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Domain.Enums;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseAccountRepository : IAccountRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseAccountRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Account>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<AccountRow>().Where(a => a.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<AccountRow>()
            .Where(a => a.UserId == userId)
            .Where(a => a.Id == id)
            .Get();
        var row = response.Models.FirstOrDefault();
        return row is null ? null : ToEntity(row);
    }

    public async Task UpdateBalanceAsync(Guid id, decimal newBalance)
    {
        var userId = _currentUser.UserId;
        await _client.From<AccountRow>()
            .Where(a => a.UserId == userId)
            .Where(a => a.Id == id)
            .Set(a => a.Balance, newBalance)
            .Update();
    }

    private static Account ToEntity(AccountRow row) => new()
    {
        Id = row.Id,
        Name = row.Name,
        Type = Enum.Parse<AccountType>(row.Type),
        Subtitle = row.Subtitle,
        Balance = row.Balance
    };
}
