using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class CuentasController : Controller
{
    private readonly IAccountService _accountService;

    public CuentasController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAccountsAsync();
        return View(accounts);
    }
}
