using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseExpenseRepository cuando existan credenciales reales.
public class DummyExpenseRepository : IExpenseRepository
{
    private static readonly object Lock = new();
    private static readonly List<Expense> Expenses = new()
    {
        new Expense { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), AccountId = Guid.Parse("00000000-0000-0000-0000-000000000001"), Category = "Comida", Amount = 180m, Date = DateTime.UtcNow.AddDays(-1) },
        new Expense { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), AccountId = Guid.Parse("00000000-0000-0000-0000-000000000001"), Category = "Transporte", Amount = 95m, Date = DateTime.UtcNow.AddDays(-2) },
        new Expense { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), AccountId = Guid.Parse("00000000-0000-0000-0000-000000000002"), Category = "Café", Amount = 68m, Date = DateTime.UtcNow.AddDays(-3) },
    };

    public Task<List<Expense>> GetRecentAsync(int count)
    {
        lock (Lock)
        {
            return Task.FromResult(Expenses.OrderByDescending(e => e.Date).Take(count).ToList());
        }
    }

    public Task<List<Expense>> GetAllAsync()
    {
        lock (Lock)
        {
            return Task.FromResult(Expenses.ToList());
        }
    }

    public Task AddAsync(Expense expense)
    {
        lock (Lock)
        {
            expense.Id = Guid.NewGuid();
            Expenses.Add(expense);
        }
        return Task.CompletedTask;
    }
}
