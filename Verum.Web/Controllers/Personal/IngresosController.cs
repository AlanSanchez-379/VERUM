using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class IngresosController : Controller
{
    private readonly IIncomeService _incomeService;

    public IngresosController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    public async Task<IActionResult> Index()
    {
        var incomes = await _incomeService.GetCurrentPeriodAsync();
        return View(incomes);
    }
}
