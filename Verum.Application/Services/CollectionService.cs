using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly IReceivableRepository _receivableRepository;

    public CollectionService(IReceivableRepository receivableRepository)
    {
        _receivableRepository = receivableRepository;
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

    public Task MarkCollectedAsync(Guid businessId, Guid id) => _receivableRepository.MarkCollectedAsync(businessId, id);

    private static ReceivableDto ToDto(Receivable r) => new(r.Id, r.BusinessId, r.ClientName, r.Amount, r.DueDate, r.IsCollected);
}
