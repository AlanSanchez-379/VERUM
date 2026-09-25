using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IIncomeRepository
{
    Task<List<Income>> GetAllForCurrentPeriodAsync();
}
