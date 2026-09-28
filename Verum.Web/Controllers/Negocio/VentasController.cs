using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class VentasController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessAccountService _accountService;

    public VentasController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        ISaleService saleService,
        IBusinessAccountService accountService)
        : base(businessService, currentBusiness)
    {
        _saleService = saleService;
        _accountService = accountService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var vm = new NegocioSalesViewModel
        {
            Business = business,
            Sales = await _saleService.GetAllAsync(business.Id),
            Accounts = await _accountService.GetAllAsync(business.Id)
        };

        return View(vm);
    }

    public class RegisterSaleRequest
    {
        public Guid AccountId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterSaleRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest(new { error = "Ingresá una descripción." });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "El monto debe ser mayor a cero." });
        }

        if (request.AccountId == Guid.Empty)
        {
            return BadRequest(new { error = "Elegí a qué cuenta entra la venta." });
        }

        await _saleService.RegisterSaleAsync(business.Id, request.AccountId, request.Description.Trim(), request.Amount);
        var total = await _saleService.GetTotalAsync(business.Id);
        return Ok(new { total });
    }
}
