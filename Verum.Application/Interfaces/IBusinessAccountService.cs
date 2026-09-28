using Verum.Application.DTOs.Negocio;

namespace Verum.Application.Interfaces;

public interface IBusinessAccountService
{
    Task<List<BusinessAccountDto>> GetAllAsync(Guid businessId);
    Task<decimal> GetTotalAvailableAsync(Guid businessId);
    Task<BusinessAccountDto> CreateAsync(Guid businessId, string name, string subtitle);

    // Cuenta por defecto para movimientos automaticos (cobros/pagos disparados
    // por otros modulos). Si el negocio no tiene ninguna cuenta todavia, crea
    // "Caja" automaticamente para que nunca falle un registro por falta de cuenta.
    Task<Guid> GetOrCreateDefaultAccountIdAsync(Guid businessId);
}
