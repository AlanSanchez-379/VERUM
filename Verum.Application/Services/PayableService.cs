using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class PayableService : IPayableService
{
    private readonly IPayableRepository _payableRepository;
    private readonly IBusinessExpenseService _expenseService;
    private readonly IBusinessAccountService _accountService;

    public PayableService(IPayableRepository payableRepository, IBusinessExpenseService expenseService, IBusinessAccountService accountService)
    {
        _payableRepository = payableRepository;
        _expenseService = expenseService;
        _accountService = accountService;
    }

    public async Task<List<PayableDto>> GetAllAsync(Guid businessId)
    {
        var payables = await _payableRepository.GetAllAsync(businessId);
        return payables.Select(ToDto).ToList();
    }

    public async Task RegisterAsync(Guid businessId, string supplierName, decimal amount, DateTime dueDate)
    {
        if (amount <= 0)
        {
            return;
        }

        await _payableRepository.AddAsync(new Payable
        {
            BusinessId = businessId,
            SupplierName = supplierName,
            Amount = amount,
            DueDate = dueDate,
            IsPaid = false
        });
    }

    public async Task<BusinessExpenseResult> MarkPaidAsync(Guid businessId, Guid id)
    {
        var payables = await _payableRepository.GetAllAsync(businessId);
        var payable = payables.FirstOrDefault(p => p.Id == id);
        if (payable is null || payable.IsPaid)
        {
            // ya pagado o no existe: no volver a restar el dinero.
            return new BusinessExpenseResult(false, "El proveedor no existe o ya está pagado.");
        }

        // El pago es plata real que sale recien ahora: se refleja como gasto,
        // no en el momento en que se registro la obligacion pendiente.
        var accountId = await _accountService.GetOrCreateDefaultAccountIdAsync(businessId);
        var result = await _expenseService.RegisterExpenseAsync(businessId, accountId, $"Proveedor: {payable.SupplierName}", payable.Amount);
        if (!result.Success)
        {
            return result;
        }

        await _payableRepository.MarkPaidAsync(businessId, id);
        return result;
    }

    private static PayableDto ToDto(Payable p) => new(p.Id, p.BusinessId, p.SupplierName, p.Amount, p.DueDate, p.IsPaid);
}
