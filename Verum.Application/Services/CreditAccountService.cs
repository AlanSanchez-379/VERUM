using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;

namespace Verum.Application.Services;

public class CreditAccountService : ICreditAccountService
{
    private readonly ICreditAccountRepository _creditAccountRepository;

    public CreditAccountService(ICreditAccountRepository creditAccountRepository)
    {
        _creditAccountRepository = creditAccountRepository;
    }

    public async Task<List<CreditAccountDto>> GetAllAsync()
    {
        var accounts = await _creditAccountRepository.GetAllAsync();
        return accounts
            .Select(c => new CreditAccountDto(c.Id, c.Name, c.CreditLimit, c.UsedAmount, c.CutoffDate, c.DueDate))
            .ToList();
    }
}
