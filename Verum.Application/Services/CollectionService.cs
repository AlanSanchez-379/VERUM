using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly IReceivableRepository _receivableRepository;
    private readonly ISaleService _saleService;
    private readonly IBusinessAccountService _accountService;

    public CollectionService(IReceivableRepository receivableRepository, ISaleService saleService, IBusinessAccountService accountService)
    {
        _receivableRepository = receivableRepository;
        _saleService = saleService;
        _accountService = accountService;
    }

    public async Task<List<ReceivableDto>> GetAllAsync(Guid businessId)
    {
        var receivables = await _receivableRepository.GetAllAsync(businessId);
        return receivables.Select(ToDto).ToList();
    }

    public async Task RegisterAsync(Guid businessId, string clientName, decimal amount, DateTime dueDate)
    {
        if (amount <= 0)
        {
            return;
        }

        await _receivableRepository.AddAsync(new Receivable
        {
            BusinessId = businessId,
            ClientName = clientName,
            Amount = amount,
            DueDate = dueDate,
            IsCollected = false
        });
    }

    public async Task MarkCollectedAsync(Guid businessId, Guid id)
    {
        var receivables = await _receivableRepository.GetAllAsync(businessId);
        var receivable = receivables.FirstOrDefault(r => r.Id == id);
        if (receivable is null || receivable.IsCollected)
        {
            // ya cobrado o no existe: no volver a sumar el dinero.
            return;
        }

        await _receivableRepository.MarkCollectedAsync(businessId, id);

        // El cobro es plata real que entra recien ahora: se refleja como venta,
        // no en el momento en que se registro la cuenta por cobrar.
        var accountId = await _accountService.GetOrCreateDefaultAccountIdAsync(businessId);
        await _saleService.RegisterSaleAsync(businessId, accountId, $"Cobro: {receivable.ClientName}", receivable.Amount);
    }

    private static ReceivableDto ToDto(Receivable r) => new(r.Id, r.BusinessId, r.ClientName, r.Amount, r.DueDate, r.IsCollected);
}
