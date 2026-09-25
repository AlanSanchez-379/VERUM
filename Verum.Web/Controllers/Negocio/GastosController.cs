using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class GastosController : NegocioBaseController
{
    private readonly IBusinessExpenseService _expenseService;

    public GastosController(IBusinessService businessService, ICurrentBusinessService currentBusiness, IBusinessExpenseService expenseService)
        : base(businessService, currentBusiness)
    {
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var expenses = await _expenseService.GetAllAsync(business.Id);
        var vm = new NegocioExpensesViewModel
        {
            Business = business,
            Expenses = expenses
        };

        return View(vm);
    }

    public class RegisterExpenseRequest
    {
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

        await _expenseService.RegisterExpenseAsync(business.Id, request.Category.Trim(), request.Amount);
        var total = await _expenseService.GetTotalAsync(business.Id);
        return Ok(new { total });
    }
}
