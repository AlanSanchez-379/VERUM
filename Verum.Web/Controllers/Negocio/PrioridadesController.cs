using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class PrioridadesController : NegocioBaseController
{
    private readonly IBusinessGoalService _goalService;

    public PrioridadesController(IBusinessService businessService, ICurrentBusinessService currentBusiness, IBusinessGoalService goalService)
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
        return View(goals);
    }

    public class UpdatePriorityRequest
    {
        public Guid GoalId { get; set; }
        public string Priority { get; set; } = string.Empty;
    }

    [HttpPost("actualizar")]
    public async Task<IActionResult> Actualizar([FromBody] UpdatePriorityRequest request)
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return BadRequest(new { error = "No hay un negocio activo." });
        }

        await _goalService.UpdatePriorityAsync(business.Id, request.GoalId, request.Priority);
        var goals = await _goalService.GetAllAsync(business.Id);
        return Ok(goals);
    }
}
