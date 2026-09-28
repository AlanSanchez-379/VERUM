using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBusinessAccountRepository _accountRepository;

    public SaleService(ISaleRepository saleRepository, IBusinessAccountRepository accountRepository)
    {
        _saleRepository = saleRepository;
        _accountRepository = accountRepository;
    }

    public async Task<List<SaleDto>> GetRecentAsync(Guid businessId, int count)
    {
        var sales = await _saleRepository.GetRecentAsync(businessId, count);
        return sales.Select(ToDto).ToList();
    }

    public async Task<List<SaleDto>> GetAllAsync(Guid businessId)
    {
        var sales = await _saleRepository.GetAllAsync(businessId);
        return sales.Select(ToDto).ToList();
    }

    public async Task<decimal> GetTotalAsync(Guid businessId)
    {
        var sales = await _saleRepository.GetAllAsync(businessId);
        return sales.Sum(s => s.Amount);
    }

    public async Task RegisterSaleAsync(Guid businessId, Guid accountId, string description, decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        var account = await _accountRepository.GetByIdAsync(businessId, accountId);
        if (account is null)
        {
            return;
        }

        await _accountRepository.UpdateBalanceAsync(businessId, accountId, account.Balance + amount);

        await _saleRepository.AddAsync(new Sale
        {
            BusinessId = businessId,
            AccountId = accountId,
            Description = description,
            Amount = amount,
            Date = DateTime.UtcNow
        });
    }

    private static SaleDto ToDto(Sale s) => new(s.Id, s.BusinessId, s.AccountId, s.Description, s.Amount, s.Date);
}
