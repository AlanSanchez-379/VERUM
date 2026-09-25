using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class InversionesController : NegocioBaseController
{
    private readonly IInvestmentService _investmentService;

    public InversionesController(IBusinessService businessService, ICurrentBusinessService currentBusiness, IInvestmentService investmentService)
        : base(businessService, currentBusiness)
    {
        _investmentService = investmentService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var investments = await _investmentService.GetAllAsync(business.Id);
        var vm = new NegocioInvestmentsViewModel
        {
            Business = business,
            Investments = investments
        };

        return View(vm);
    }

    public class RegisterInvestmentRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterInvestmentRequest request)
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

        await _investmentService.RegisterAsync(business.Id, request.Description.Trim(), request.Amount);
        return Ok();
    }
}
