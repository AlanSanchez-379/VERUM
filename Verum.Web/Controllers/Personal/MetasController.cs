using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class MetasController : Controller
{
    private readonly IGoalService _goalService;

    public MetasController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        var goals = await _goalService.GetAllAsync();
        return View(goals);
    }
}
