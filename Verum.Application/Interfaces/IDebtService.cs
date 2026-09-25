using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IDebtService
{
    Task<List<DebtDto>> GetAllAsync();
}
