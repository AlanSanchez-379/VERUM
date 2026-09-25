using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IIncomeService
{
    Task<List<IncomeDto>> GetCurrentPeriodAsync();
    Task<decimal> GetTotalReceivedAsync();
}
