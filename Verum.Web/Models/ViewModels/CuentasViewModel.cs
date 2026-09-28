using Verum.Application.DTOs.Personal;

namespace Verum.Web.Models.ViewModels;

public class CuentasViewModel
{
    public List<AccountDto> Accounts { get; set; } = new();
    public List<CreditAccountDto> CreditAccounts { get; set; } = new();
    public List<DebtDto> Debts { get; set; } = new();

    public decimal TotalAvailable => Accounts.Sum(a => a.Balance);
    public decimal TotalCreditAvailable => CreditAccounts.Sum(c => c.Available);
    public decimal TotalDebtRemaining => Debts.Sum(d => d.RemainingAmount);
}
