using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseCommitmentRepository cuando existan credenciales reales.
public class DummyCommitmentRepository : ICommitmentRepository
{
    private static readonly List<Commitment> Commitments = new()
    {
        new Commitment { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Tarjeta BBVA", Amount = 2000m, DueDate = DateTime.UtcNow.AddDays(2), IsPaid = true },
        new Commitment { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "Internet", Amount = 600m, DueDate = DateTime.UtcNow.AddDays(5), IsPaid = false },
        new Commitment { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Name = "Gym", Amount = 500m, DueDate = DateTime.UtcNow.AddDays(8), IsPaid = false },
        new Commitment { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Name = "Netflix", Amount = 299m, DueDate = DateTime.UtcNow.AddDays(10), IsPaid = false },
    };

    public Task<List<Commitment>> GetAllForCurrentPeriodAsync() => Task.FromResult(Commitments.ToList());
}
