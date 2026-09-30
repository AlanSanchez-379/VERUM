using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers;

[Route("configuracion")]
public class ConfiguracionController : Controller
{
    private readonly global::Supabase.Client _client;

    public ConfiguracionController(global::Supabase.Client client)
    {
        _client = client;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Email"] = _client.Auth.CurrentUser?.Email ?? string.Empty;
        return View();
    }
}
