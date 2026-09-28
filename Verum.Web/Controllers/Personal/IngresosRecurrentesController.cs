using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Personal;

[ApiController]
[Route("personal/ingresos-recurrentes")]
public class IngresosRecurrentesController : Controller
{
    private readonly IRecurringIncomeService _recurringIncomeService;
    private readonly IAccountService _accountService;

    public IngresosRecurrentesController(IRecurringIncomeService recurringIncomeService, IAccountService accountService)
    {
        _recurringIncomeService = recurringIncomeService;
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new RecurringIncomesViewModel
        {
            Patterns = await _recurringIncomeService.GetAllAsync(),
            Pending = await _recurringIncomeService.GetPendingConfirmationsAsync()
        };

        return View(vm);
    }

    public class RegisterPatternRequest
    {
        public string Source { get; set; } = string.Empty;
        public decimal ExpectedAmount { get; set; }
        public int DayOfMonth { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterPatternRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Source))
        {
            return BadRequest(new { error = "Ingresá un nombre." });
        }

        if (request.ExpectedAmount <= 0)
        {
            return BadRequest(new { error = "El monto esperado debe ser mayor a cero." });
        }

        if (request.DayOfMonth is < 1 or > 28)
        {
            return BadRequest(new { error = "El día del mes debe ser entre 1 y 28." });
        }

        await _recurringIncomeService.RegisterPatternAsync(request.Source.Trim(), request.ExpectedAmount, request.DayOfMonth);
        return Ok();
    }

    public class ConfirmRequest
    {
        public Guid? AccountId { get; set; }
        public decimal ActualAmount { get; set; }
    }

    [HttpPost("confirmar/{recurringIncomeId}")]
    public async Task<IActionResult> Confirmar(Guid recurringIncomeId, [FromBody] ConfirmRequest request)
    {
        var result = await _recurringIncomeService.ConfirmAsync(recurringIncomeId, request.AccountId, request.ActualAmount);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new
        {
            totalAvailable = result.TotalAvailable,
            accountBalance = result.AccountBalance
        });
    }
}
