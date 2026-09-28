using Verum.Application.DTOs.Negocio;
using Verum.Domain.Entities;

namespace Verum.Application.Interfaces;

public interface ITransferService
{
    Task<List<TransferDto>> GetRecentAsync(Guid businessId, int take = 10);
    Task<TransferResult> TransferAsync(Guid businessId, Guid personalAccountId, Guid businessAccountId, TransferDirection direction, decimal amount, string note);
}
