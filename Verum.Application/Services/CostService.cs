using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class CostService : ICostService
{
    private readonly ICostRepository _costRepository;

    public CostService(ICostRepository costRepository)
    {
        _costRepository = costRepository;
    }

    public async Task<List<CostDto>> GetAllAsync(Guid businessId)
    {
        var costs = await _costRepository.GetAllAsync(businessId);
        return costs.Select(ToDto).ToList();
    }

    public async Task RegisterAsync(Guid businessId, string name, decimal amount, DateTime dueDate)
    {
        if (amount <= 0)
        {
            return;
        }

        await _costRepository.AddAsync(new Cost
        {
            BusinessId = businessId,
            Name = name,
            Amount = amount,
            DueDate = dueDate,
            IsPaid = false
        });
    }

    public Task MarkPaidAsync(Guid businessId, Guid id) => _costRepository.MarkPaidAsync(businessId, id);

    private static CostDto ToDto(Cost c) => new(c.Id, c.BusinessId, c.Name, c.Amount, c.DueDate, c.IsPaid);
}
