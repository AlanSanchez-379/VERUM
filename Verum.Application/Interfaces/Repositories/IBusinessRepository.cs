using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IBusinessRepository
{
    Task<List<Business>> GetAllAsync();
    Task<Business?> GetByIdAsync(Guid id);
    Task<Business> CreateAsync(Business business);
}
