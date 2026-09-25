using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers.Transversal;

[Route("api/[controller]")]
public class NotificacionesController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new { });
    }
}
