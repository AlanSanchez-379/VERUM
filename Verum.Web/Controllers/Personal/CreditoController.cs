using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class CreditoController : Controller
{
    private readonly ICreditAccountService _creditAccountService;

    public CreditoController(ICreditAccountService creditAccountService)
    {
        _creditAccountService = creditAccountService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _creditAccountService.GetAllAsync();
        return View(accounts);
    }
}
