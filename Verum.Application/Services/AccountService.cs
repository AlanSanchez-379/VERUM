using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;

namespace Verum.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<List<AccountDto>> GetAccountsAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts
            .Select(a => new AccountDto(a.Id, a.Name, a.Subtitle, a.Balance))
            .ToList();
    }

    public async Task<decimal> GetTotalAvailableAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Sum(a => a.Balance);
    }
}
