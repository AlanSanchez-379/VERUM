using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers;

[AllowAnonymous]
[Route("error")]
public class ErrorStatusController : Controller
{
    [HttpGet("{code:int}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(int code)
    {
        Response.StatusCode = code;
        ViewData["Code"] = code;
        return View();
    }
}
