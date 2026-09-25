using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class TaxService : ITaxService
{
    private readonly ITaxRepository _taxRepository;

    public TaxService(ITaxRepository taxRepository)
    {
        _taxRepository = taxRepository;
    }

    public async Task<List<TaxDto>> GetAllAsync(Guid businessId)
    {
        var taxes = await _taxRepository.GetAllAsync(businessId);
        return taxes.Select(ToDto).ToList();
    }

    public async Task RegisterAsync(Guid businessId, string name, decimal amount, DateTime dueDate)
    {
        if (amount <= 0)
        {
            return;
        }

        await _taxRepository.AddAsync(new Tax
        {
            BusinessId = businessId,
            Name = name,
            Amount = amount,
            DueDate = dueDate,
            IsPaid = false
        });
    }

    public Task MarkPaidAsync(Guid businessId, Guid id) => _taxRepository.MarkPaidAsync(businessId, id);

    private static TaxDto ToDto(Tax t) => new(t.Id, t.BusinessId, t.Name, t.Amount, t.DueDate, t.IsPaid);
}
