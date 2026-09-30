using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class TaxService : ITaxService
{
    private readonly ITaxRepository _taxRepository;
    private readonly IBusinessExpenseService _expenseService;
    private readonly IBusinessAccountService _accountService;

    public TaxService(ITaxRepository taxRepository, IBusinessExpenseService expenseService, IBusinessAccountService accountService)
    {
        _taxRepository = taxRepository;
        _expenseService = expenseService;
        _accountService = accountService;
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

    public async Task<BusinessExpenseResult> MarkPaidAsync(Guid businessId, Guid id)
    {
        var taxes = await _taxRepository.GetAllAsync(businessId);
        var tax = taxes.FirstOrDefault(t => t.Id == id);
        if (tax is null || tax.IsPaid)
        {
            return new BusinessExpenseResult(false, "El impuesto no existe o ya está pagado.");
        }

        var accountId = await _accountService.GetOrCreateDefaultAccountIdAsync(businessId);
        var result = await _expenseService.RegisterExpenseAsync(businessId, accountId, $"Impuesto: {tax.Name}", tax.Amount);
        if (!result.Success)
        {
            return result;
        }

        await _taxRepository.MarkPaidAsync(businessId, id);
        return result;
    }

    private static TaxDto ToDto(Tax t) => new(t.Id, t.BusinessId, t.Name, t.Amount, t.DueDate, t.IsPaid);
}
