using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface IAccountService
{
    Task<List<AccountDto>> GetAccountsAsync();
    Task<decimal> GetTotalAvailableAsync();
}
