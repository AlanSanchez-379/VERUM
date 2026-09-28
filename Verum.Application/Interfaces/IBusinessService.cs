using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface IBusinessService
{
    Task<List<BusinessDto>> GetAllAsync();
    Task<BusinessDto?> GetByIdAsync(Guid id);
    Task<BusinessDto> CreateAsync(string name, string industry);
    Task RenameAsync(Guid id, string name);
}
