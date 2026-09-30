using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers;

[Route("guia")]
public class GuiaController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
