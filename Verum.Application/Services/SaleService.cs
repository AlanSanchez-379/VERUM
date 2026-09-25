using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;

    public SaleService(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
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

    public async Task RegisterSaleAsync(Guid businessId, string description, decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        await _saleRepository.AddAsync(new Sale
        {
            BusinessId = businessId,
            Description = description,
            Amount = amount,
            Date = DateTime.UtcNow
        });
    }

    private static SaleDto ToDto(Sale s) => new(s.Id, s.BusinessId, s.Description, s.Amount, s.Date);
}
