using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class MetasController : Controller
{
    private const long MaxUploadBytes = 4 * 1024 * 1024; // margen extra sobre el limite real del servicio

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

    [HttpPost("{id:guid}/imagen")]
    [RequestSizeLimit(MaxUploadBytes)]
    public async Task<IActionResult> SubirImagen(Guid id, IFormFile? imagen)
    {
        if (imagen is null || imagen.Length == 0)
        {
            return BadRequest(new { error = "No se recibió ninguna imagen." });
        }

        if (imagen.Length > MaxUploadBytes)
        {
            return BadRequest(new { error = "La imagen es muy pesada." });
        }

        using var stream = new MemoryStream();
        await imagen.CopyToAsync(stream);

        var result = await _goalService.SetImageAsync(id, stream.ToArray(), imagen.ContentType);
        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { imageUrl = result.ImageUrl });
    }

    [HttpPost("{id:guid}/imagen/eliminar")]
    public async Task<IActionResult> EliminarImagen(Guid id)
    {
        await _goalService.RemoveImageAsync(id);
        return Ok();
    }
}
