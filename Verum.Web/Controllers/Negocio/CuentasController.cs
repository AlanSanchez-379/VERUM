using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class CuentasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
