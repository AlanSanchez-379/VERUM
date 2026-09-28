using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class BusinessService : IBusinessService
{
    private readonly IBusinessRepository _businessRepository;
    private readonly IBusinessAccountService _accountService;

    public BusinessService(IBusinessRepository businessRepository, IBusinessAccountService accountService)
    {
        _businessRepository = businessRepository;
        _accountService = accountService;
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

        // Todo negocio nace con una cuenta real donde entra/sale su dinero.
        await _accountService.CreateAsync(created.Id, "Caja", "Cuenta principal");

        return ToDto(created);
    }

    public Task RenameAsync(Guid id, string name) => _businessRepository.RenameAsync(id, name);

    private static BusinessDto ToDto(Business b) => new(b.Id, b.Name, b.Industry, b.CreatedAt);
}
