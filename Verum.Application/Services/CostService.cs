using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class CostService : ICostService
{
    private readonly ICostRepository _costRepository;
    private readonly IBusinessExpenseService _expenseService;
    private readonly IBusinessAccountService _accountService;

    public CostService(ICostRepository costRepository, IBusinessExpenseService expenseService, IBusinessAccountService accountService)
    {
        _costRepository = costRepository;
        _expenseService = expenseService;
        _accountService = accountService;
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

    public async Task MarkPaidAsync(Guid businessId, Guid id)
    {
        var costs = await _costRepository.GetAllAsync(businessId);
        var cost = costs.FirstOrDefault(c => c.Id == id);
        if (cost is null || cost.IsPaid)
        {
            return;
        }

        await _costRepository.MarkPaidAsync(businessId, id);
        var accountId = await _accountService.GetOrCreateDefaultAccountIdAsync(businessId);
        await _expenseService.RegisterExpenseAsync(businessId, accountId, $"Costo fijo: {cost.Name}", cost.Amount);
    }

    private static CostDto ToDto(Cost c) => new(c.Id, c.BusinessId, c.Name, c.Amount, c.DueDate, c.IsPaid);
}
