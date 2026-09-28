using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseIncomeRepository cuando existan credenciales reales.
public class DummyIncomeRepository : IIncomeRepository
{
    private static readonly object Lock = new();
    private static readonly List<Income> Incomes = new()
    {
        new Income
        {
            Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
            AccountId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Source = "Sueldo",
            Amount = 5000m,
            ExpectedDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 5),
            IsReceived = true
        }
    };

    public Task<List<Income>> GetAllForCurrentPeriodAsync() => Task.FromResult(Incomes.ToList());

    public Task AddAsync(Income income)
    {
        lock (Lock)
        {
            income.Id = Guid.NewGuid();
            Incomes.Add(income);
        }
        return Task.CompletedTask;
    }
}
