using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IRecurringIncomeRepository
{
    Task<List<RecurringIncome>> GetAllAsync();
    Task AddAsync(RecurringIncome recurringIncome);

    Task<List<RecurringIncomeConfirmation>> GetConfirmationsForPeriodAsync(DateTime period);
    Task AddConfirmationAsync(RecurringIncomeConfirmation confirmation);
}
