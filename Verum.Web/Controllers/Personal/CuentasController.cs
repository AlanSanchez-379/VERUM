using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class CuentasController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ICreditAccountService _creditAccountService;
    private readonly IDebtService _debtService;

    public CuentasController(IAccountService accountService, ICreditAccountService creditAccountService, IDebtService debtService)
    {
        _accountService = accountService;
        _creditAccountService = creditAccountService;
        _debtService = debtService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new CuentasViewModel
        {
            Accounts = await _accountService.GetAccountsAsync(),
            CreditAccounts = await _creditAccountService.GetAllAsync(),
            Debts = await _debtService.GetAllAsync()
        };

        return View(vm);
    }
}
