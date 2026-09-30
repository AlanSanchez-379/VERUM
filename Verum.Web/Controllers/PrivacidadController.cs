using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers;

[AllowAnonymous]
[Route("privacidad")]
public class PrivacidadController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();
}
