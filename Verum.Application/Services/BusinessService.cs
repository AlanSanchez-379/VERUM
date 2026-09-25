using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class BusinessService : IBusinessService
{
    private readonly IBusinessRepository _businessRepository;

    public BusinessService(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<List<BusinessDto>> GetAllAsync()
    {
        var businesses = await _businessRepository.GetAllAsync();
        return businesses.Select(ToDto).ToList();
    }

    public async Task<BusinessDto?> GetByIdAsync(Guid id)
    {
        var business = await _businessRepository.GetByIdAsync(id);
        return business is null ? null : ToDto(business);
    }

    public async Task<BusinessDto> CreateAsync(string name, string industry)
    {
        var created = await _businessRepository.CreateAsync(new Business
        {
            Name = name,
            Industry = industry,
            CreatedAt = DateTime.UtcNow
        });

        return ToDto(created);
    }

    private static BusinessDto ToDto(Business b) => new(b.Id, b.Name, b.Industry, b.CreatedAt);
}
