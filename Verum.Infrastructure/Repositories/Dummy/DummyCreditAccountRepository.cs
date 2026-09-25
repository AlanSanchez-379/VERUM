using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseCreditAccountRepository cuando existan credenciales reales.
public class DummyCreditAccountRepository : ICreditAccountRepository
{
    private static readonly List<CreditAccount> CreditAccounts = new()
    {
        new CreditAccount
        {
            Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
            Name = "BBVA Oro",
            CreditLimit = 25000m,
            UsedAmount = 6200m,
            CutoffDate = DateTime.UtcNow.AddDays(9),
            DueDate = DateTime.UtcNow.AddDays(24)
        }
    };

    public Task<List<CreditAccount>> GetAllAsync() => Task.FromResult(CreditAccounts.ToList());
}
