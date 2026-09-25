using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class CostosController : NegocioBaseController
{
    private readonly ICostService _costService;

    public CostosController(IBusinessService businessService, ICurrentBusinessService currentBusiness, ICostService costService)
        : base(businessService, currentBusiness)
    {
        _costService = costService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var costs = await _costService.GetAllAsync(business.Id);
        var vm = new NegocioCostsViewModel
        {
            Business = business,
            Costs = costs
        };

        return View(vm);
    }

    public class RegisterCostRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterCostRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { error = "Ingresá un nombre." });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "El monto debe ser mayor a cero." });
        }

        if (request.DueDate == default)
        {
            return BadRequest(new { error = "Ingresá una fecha de vencimiento." });
        }

        await _costService.RegisterAsync(business.Id, request.Name.Trim(), request.Amount, request.DueDate);
        return Ok();
    }

    [HttpPost("pagar/{id}")]
    public async Task<IActionResult> Pagar(Guid id)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        await _costService.MarkPaidAsync(business.Id, id);
        return Ok();
    }
}
