using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseDebtRepository cuando existan credenciales reales.
public class DummyDebtRepository : IDebtRepository
{
    private static readonly List<Debt> Debts = new()
    {
        new Debt
        {
            Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            Name = "Préstamo personal",
            TotalAmount = 20000m,
            RemainingAmount = 12500m,
            MonthlyPayment = 1100m,
            NextDueDate = DateTime.UtcNow.AddDays(12)
        }
    };

    public Task<List<Debt>> GetAllAsync() => Task.FromResult(Debts.ToList());
}
