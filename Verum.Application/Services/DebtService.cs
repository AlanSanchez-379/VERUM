using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;

namespace Verum.Application.Services;

public class DebtService : IDebtService
{
    private readonly IDebtRepository _debtRepository;

    public DebtService(IDebtRepository debtRepository)
    {
        _debtRepository = debtRepository;
    }

    public async Task<List<DebtDto>> GetAllAsync()
    {
        var debts = await _debtRepository.GetAllAsync();
        return debts
            .Select(d => new DebtDto(d.Id, d.Name, d.TotalAmount, d.RemainingAmount, d.MonthlyPayment, d.NextDueDate))
            .ToList();
    }
}
