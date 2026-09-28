using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class DeudasController : Controller
{
    // Deudas se unificó dentro de /personal/cuentas.
    public IActionResult Index() => RedirectPermanent("/personal/cuentas");
}
