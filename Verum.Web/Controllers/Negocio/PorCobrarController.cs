using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class PorCobrarController : NegocioBaseController
{
    private readonly ICollectionService _collectionService;

    public PorCobrarController(IBusinessService businessService, ICurrentBusinessService currentBusiness, ICollectionService collectionService)
        : base(businessService, currentBusiness)
    {
        _collectionService = collectionService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var receivables = await _collectionService.GetAllAsync(business.Id);
        var vm = new NegocioReceivablesViewModel
        {
            Business = business,
            Receivables = receivables
        };

        return View(vm);
    }

    public class RegisterReceivableRequest
    {
        public string ClientName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterReceivableRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (string.IsNullOrWhiteSpace(request.ClientName))
        {
            return BadRequest(new { error = "Ingresá el nombre del cliente." });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "El monto debe ser mayor a cero." });
        }

        if (request.DueDate == default)
        {
            return BadRequest(new { error = "Ingresá una fecha de vencimiento." });
        }

        await _collectionService.RegisterAsync(business.Id, request.ClientName.Trim(), request.Amount, request.DueDate);
        return Ok();
    }

    [HttpPost("cobrar/{id}")]
    public async Task<IActionResult> Cobrar(Guid id)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        await _collectionService.MarkCollectedAsync(business.Id, id);
        return Ok();
    }
}
