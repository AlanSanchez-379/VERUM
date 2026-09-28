using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Infrastructure.Repositories.Dummy;

// Implementacion dummy en memoria. Reemplazar por SupabaseBusinessRepository cuando existan credenciales reales.
public class DummyBusinessRepository : IBusinessRepository
{
    private static readonly object Lock = new();
    private static readonly List<Business> Businesses = new();

    public Task<List<Business>> GetAllAsync()
    {
        lock (Lock)
        {
            return Task.FromResult(Businesses.ToList());
        }
    }

    public Task<Business?> GetByIdAsync(Guid id)
    {
        lock (Lock)
        {
            return Task.FromResult(Businesses.FirstOrDefault(b => b.Id == id));
        }
    }

    public Task<Business> CreateAsync(Business business)
    {
        lock (Lock)
        {
            business.Id = Guid.NewGuid();
            Businesses.Add(business);
            return Task.FromResult(business);
        }
    }

    public Task RenameAsync(Guid id, string name)
    {
        lock (Lock)
        {
            var business = Businesses.FirstOrDefault(b => b.Id == id);
            if (business is not null)
            {
                business.Name = name;
            }
        }
        return Task.CompletedTask;
    }
}
