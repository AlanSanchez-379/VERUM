using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;

namespace Verum.Application.Services;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomeService(IIncomeRepository incomeRepository)
    {
        _incomeRepository = incomeRepository;
    }

    public async Task<List<IncomeDto>> GetCurrentPeriodAsync()
    {
        var incomes = await _incomeRepository.GetAllForCurrentPeriodAsync();
        return incomes
            .Select(i => new IncomeDto(i.Id, i.Source, i.Amount, i.ExpectedDate, i.IsReceived))
            .ToList();
    }

    public async Task<decimal> GetTotalReceivedAsync()
    {
        var incomes = await _incomeRepository.GetAllForCurrentPeriodAsync();
        return incomes.Where(i => i.IsReceived).Sum(i => i.Amount);
    }
}
