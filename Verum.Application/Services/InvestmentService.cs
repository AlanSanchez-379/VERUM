using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class InvestmentService : IInvestmentService
{
    private readonly IInvestmentRepository _investmentRepository;

    public InvestmentService(IInvestmentRepository investmentRepository)
    {
        _investmentRepository = investmentRepository;
    }

    public async Task<List<InvestmentDto>> GetAllAsync(Guid businessId)
    {
        var investments = await _investmentRepository.GetAllAsync(businessId);
        return investments.Select(ToDto).ToList();
    }

    public async Task<decimal> GetTotalAsync(Guid businessId)
    {
        var investments = await _investmentRepository.GetAllAsync(businessId);
        return investments.Sum(i => i.Amount);
    }

    public async Task RegisterAsync(Guid businessId, string description, decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        await _investmentRepository.AddAsync(new Investment
        {
            BusinessId = businessId,
            Description = description,
            Amount = amount,
            Date = DateTime.UtcNow
        });
    }

    private static InvestmentDto ToDto(Investment i) => new(i.Id, i.BusinessId, i.Description, i.Amount, i.Date);
}
