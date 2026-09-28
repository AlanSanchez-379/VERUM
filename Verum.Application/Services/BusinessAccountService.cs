using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class BusinessAccountService : IBusinessAccountService
{
    private readonly IBusinessAccountRepository _accountRepository;

    public BusinessAccountService(IBusinessAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<List<BusinessAccountDto>> GetAllAsync(Guid businessId)
    {
        var accounts = await _accountRepository.GetAllAsync(businessId);
        return accounts.Select(ToDto).ToList();
    }

    public async Task<decimal> GetTotalAvailableAsync(Guid businessId)
    {
        var accounts = await _accountRepository.GetAllAsync(businessId);
        return accounts.Sum(a => a.Balance);
    }

    public async Task<BusinessAccountDto> CreateAsync(Guid businessId, string name, string subtitle)
    {
        var created = await _accountRepository.CreateAsync(new BusinessAccount
        {
            BusinessId = businessId,
            Name = name,
            Subtitle = subtitle,
            Balance = 0
        });

        return ToDto(created);
    }

    public async Task<Guid> GetOrCreateDefaultAccountIdAsync(Guid businessId)
    {
        var accounts = await _accountRepository.GetAllAsync(businessId);
        if (accounts.Count > 0)
        {
            return accounts[0].Id;
        }

        var created = await CreateAsync(businessId, "Caja", "Cuenta principal");
        return created.Id;
    }

    private static BusinessAccountDto ToDto(BusinessAccount a) => new(a.Id, a.BusinessId, a.Name, a.Subtitle, a.Balance);
}
