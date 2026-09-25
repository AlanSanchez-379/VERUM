using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class MetasController : NegocioBaseController
{
    private readonly IBusinessGoalService _goalService;

    public MetasController(IBusinessService businessService, ICurrentBusinessService currentBusiness, IBusinessGoalService goalService)
        : base(businessService, currentBusiness)
    {
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var goals = await _goalService.GetAllAsync(business.Id);
        ViewData["BusinessName"] = business.Name;
        return View(goals);
    }

    public class RegisterGoalRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public DateTime TargetDate { get; set; }
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegisterGoalRequest request)
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

        if (request.TargetAmount <= 0)
        {
            return BadRequest(new { error = "El monto objetivo debe ser mayor a cero." });
        }

        if (request.TargetDate == default)
        {
            return BadRequest(new { error = "Ingresá una fecha objetivo." });
        }

        await _goalService.RegisterAsync(business.Id, request.Name.Trim(), request.TargetAmount, request.TargetDate);
        return Ok();
    }

    public class ContributeRequest
    {
        public decimal Amount { get; set; }
    }

    [HttpPost("aportar/{id}")]
    public async Task<IActionResult> Aportar(Guid id, [FromBody] ContributeRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "El monto debe ser mayor a cero." });
        }

        await _goalService.AddContributionAsync(business.Id, id, request.Amount);
        return Ok();
    }
}
