using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[ApiController]
[Route("personal/[controller]")]
public class PrioridadesController : Controller
{
    private readonly IGoalService _goalService;

    public PrioridadesController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        var goals = await _goalService.GetAllAsync();
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
        await _goalService.UpdatePriorityAsync(request.GoalId, request.Priority);
        var goals = await _goalService.GetAllAsync();
        return Ok(goals);
    }
}
