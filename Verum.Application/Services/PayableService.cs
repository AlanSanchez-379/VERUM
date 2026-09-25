using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class PayableService : IPayableService
{
    private readonly IPayableRepository _payableRepository;

    public PayableService(IPayableRepository payableRepository)
    {
        _payableRepository = payableRepository;
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

    public Task MarkPaidAsync(Guid businessId, Guid id) => _payableRepository.MarkPaidAsync(businessId, id);

    private static PayableDto ToDto(Payable p) => new(p.Id, p.BusinessId, p.SupplierName, p.Amount, p.DueDate, p.IsPaid);
}
