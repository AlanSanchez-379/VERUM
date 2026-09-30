using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class VentasController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessAccountService _accountService;
    private readonly ICollectionService _collectionService;

    public VentasController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        ISaleService saleService,
        IBusinessAccountService accountService,
        ICollectionService collectionService)
        : base(businessService, currentBusiness)
    {
        _saleService = saleService;
        _accountService = accountService;
        _collectionService = collectionService;
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
        public bool IsCredit { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
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

        if (request.IsCredit)
        {
            // Venta a crédito: todavía no entró plata real. Se registra como
            // cuenta por cobrar y solo cuando se cobre (en Por Cobrar) se
            // convierte en una venta real que mueve el saldo de la cuenta.
            if (string.IsNullOrWhiteSpace(request.ClientName))
            {
                return BadRequest(new { error = "Ingresá el nombre del cliente." });
            }

            if (request.DueDate is null)
            {
                return BadRequest(new { error = "Ingresá la fecha en que se cobra." });
            }

            await _collectionService.RegisterAsync(business.Id, request.ClientName.Trim(), request.Amount, request.DueDate.Value);
            var totalUnchanged = await _saleService.GetTotalAsync(business.Id);
            return Ok(new { total = totalUnchanged, credit = true });
        }

        if (request.AccountId == Guid.Empty)
        {
            return BadRequest(new { error = "Elegí a qué cuenta entra la venta." });
        }

        await _saleService.RegisterSaleAsync(business.Id, request.AccountId, request.Description.Trim(), request.Amount);
        var total = await _saleService.GetTotalAsync(business.Id);
        return Ok(new { total, credit = false });
    }
}
