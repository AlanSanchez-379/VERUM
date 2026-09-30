using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class ImpuestosController : NegocioBaseController
{
    private readonly ITaxService _taxService;

    public ImpuestosController(IBusinessService businessService, ICurrentBusinessService currentBusiness, ITaxService taxService)
        : base(businessService, currentBusiness)
    {
        _taxService = taxService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var taxes = await _taxService.GetAllAsync(business.Id);
        var vm = new NegocioTaxesViewModel
        {
            Business = business,
            Taxes = taxes
        };

        return View(vm);
    }

    public class RegisterTaxRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterTaxRequest request)
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

        await _taxService.RegisterAsync(business.Id, request.Name.Trim(), request.Amount, request.DueDate);
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

        var result = await _taxService.MarkPaidAsync(business.Id, id);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok();
    }
}
