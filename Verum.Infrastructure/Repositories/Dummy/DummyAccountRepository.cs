using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Domain.Enums;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria (compartida entre requests via campo estatico).
// Reemplazar por SupabaseAccountRepository cuando existan credenciales reales.
public class DummyAccountRepository : IAccountRepository
{
    private static readonly object Lock = new();

    private static readonly List<Account> Accounts = new()
    {
        new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "BBVA", Type = AccountType.Banco, Subtitle = "Banco", Balance = 8500m },
        new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Nu", Type = AccountType.Banco, Subtitle = "Banco", Balance = 3200m },
        new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Efectivo", Type = AccountType.Efectivo, Subtitle = "En mano", Balance = 1000m },
        new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Mercado Pago", Type = AccountType.Billetera, Subtitle = "Saldo bajo", Balance = 500m },
    };

    public Task<List<Account>> GetAllAsync()
    {
        lock (Lock)
        {
            return Task.FromResult(Accounts.Select(Clone).ToList());
        }
    }

    public Task<Account?> GetByIdAsync(Guid id)
    {
        lock (Lock)
        {
            var account = Accounts.FirstOrDefault(a => a.Id == id);
            return Task.FromResult(account is null ? null : Clone(account));
        }
    }

    public Task UpdateBalanceAsync(Guid id, decimal newBalance)
    {
        lock (Lock)
        {
            var account = Accounts.FirstOrDefault(a => a.Id == id);
            if (account is not null)
            {
                account.Balance = newBalance;
            }
        }
        return Task.CompletedTask;
    }

    private static Account Clone(Account a) => new()
    {
        Id = a.Id,
        Name = a.Name,
        Type = a.Type,
        Subtitle = a.Subtitle,
        Balance = a.Balance
    };
}
