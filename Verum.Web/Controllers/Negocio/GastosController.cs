using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class GastosController : NegocioBaseController
{
    private readonly IBusinessExpenseService _expenseService;
    private readonly IBusinessAccountService _accountService;

    public GastosController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        IBusinessExpenseService expenseService,
        IBusinessAccountService accountService)
        : base(businessService, currentBusiness)
    {
        _expenseService = expenseService;
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var vm = new NegocioExpensesViewModel
        {
            Business = business,
            Expenses = await _expenseService.GetAllAsync(business.Id),
            Accounts = await _accountService.GetAllAsync(business.Id)
        };

        return View(vm);
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
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (string.IsNullOrWhiteSpace(request.Category))
        {
            return BadRequest(new { error = "Ingresá una categoría." });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "El monto debe ser mayor a cero." });
        }

        if (request.AccountId == Guid.Empty)
        {
            return BadRequest(new { error = "Elegí de qué cuenta sale el gasto." });
        }

        var result = await _expenseService.RegisterExpenseAsync(business.Id, request.AccountId, request.Category.Trim(), request.Amount);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        var total = await _expenseService.GetTotalAsync(business.Id);
        return Ok(new { total });
    }
}
