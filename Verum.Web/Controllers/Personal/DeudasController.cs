using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class DeudasController : Controller
{
    private readonly IDebtService _debtService;

    public DeudasController(IDebtService debtService)
    {
        _debtService = debtService;
    }

    public async Task<IActionResult> Index()
    {
        var debts = await _debtService.GetAllAsync();
        return View(debts);
    }
}
