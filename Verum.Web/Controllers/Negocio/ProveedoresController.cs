using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class ProveedoresController : NegocioBaseController
{
    private readonly IPayableService _payableService;

    public ProveedoresController(IBusinessService businessService, ICurrentBusinessService currentBusiness, IPayableService payableService)
        : base(businessService, currentBusiness)
    {
        _payableService = payableService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var payables = await _payableService.GetAllAsync(business.Id);
        var vm = new NegocioPayablesViewModel
        {
            Business = business,
            Payables = payables
        };

        return View(vm);
    }

    public class RegisterPayableRequest
    {
        public string SupplierName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterPayableRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (string.IsNullOrWhiteSpace(request.SupplierName))
        {
            return BadRequest(new { error = "Ingresá el nombre del proveedor." });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "El monto debe ser mayor a cero." });
        }

        if (request.DueDate == default)
        {
            return BadRequest(new { error = "Ingresá una fecha de vencimiento." });
        }

        await _payableService.RegisterAsync(business.Id, request.SupplierName.Trim(), request.Amount, request.DueDate);
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

        await _payableService.MarkPaidAsync(business.Id, id);
        return Ok();
    }
}
