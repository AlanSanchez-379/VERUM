using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IRecurringIncomeService
{
    Task<List<RecurringIncomeDto>> GetAllAsync();
    Task RegisterPatternAsync(string source, decimal expectedAmount, int dayOfMonth);

    Task<List<PendingIncomeConfirmationDto>> GetPendingConfirmationsAsync();
    Task<RegisterIncomeResult> ConfirmAsync(Guid recurringIncomeId, Guid? accountId, decimal actualAmount);
}
