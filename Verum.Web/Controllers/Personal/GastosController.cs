using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[ApiController]
[Route("personal/[controller]")]
public class GastosController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IExpenseService _expenseService;

    public GastosController(IAccountService accountService, IExpenseService expenseService)
    {
        _accountService = accountService;
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAccountsAsync();
        return View(accounts);
    }

    public class RegisterExpenseRequest
    {
        public Guid AccountId { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterExpenseRequest request)
    {
        var result = await _expenseService.RegisterExpenseAsync(request.AccountId, request.Category, request.Amount);
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
