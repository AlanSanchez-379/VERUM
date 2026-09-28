using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IIncomeService
{
    Task<List<IncomeDto>> GetCurrentPeriodAsync();
    Task<decimal> GetTotalReceivedAsync();
    Task<RegisterIncomeResult> RegisterIncomeAsync(Guid accountId, string source, decimal amount);
}
