using Microsoft.AspNetCore.Mvc;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class CreditoController : Controller
{
    // Crédito se unificó dentro de /personal/cuentas.
    public IActionResult Index() => RedirectPermanent("/personal/cuentas");
}
