using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class CompromisosController : Controller
{
    private readonly ICommitmentService _commitmentService;

    public CompromisosController(ICommitmentService commitmentService)
    {
        _commitmentService = commitmentService;
    }

    public async Task<IActionResult> Index()
    {
        var commitments = await _commitmentService.GetCurrentPeriodAsync();
        return View(commitments);
    }
}
