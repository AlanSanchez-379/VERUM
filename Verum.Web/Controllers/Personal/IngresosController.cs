using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[ApiController]
[Route("personal/[controller]")]
public class IngresosController : Controller
{
    private readonly IIncomeService _incomeService;
    private readonly IAccountService _accountService;

    public IngresosController(IIncomeService incomeService, IAccountService accountService)
    {
        _incomeService = incomeService;
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAccountsAsync();
        return View(accounts);
    }

    public class RegisterIncomeRequest
    {
        public Guid AccountId { get; set; }
        public string Source { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterIncomeRequest request)
    {
        var result = await _incomeService.RegisterIncomeAsync(request.AccountId, request.Source, request.Amount);
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
