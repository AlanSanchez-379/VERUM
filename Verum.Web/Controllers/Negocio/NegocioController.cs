using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Services;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio")]
public class NegocioController : Controller
{
    private readonly IBusinessService _businessService;

    public NegocioController(IBusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var businesses = await _businessService.GetAllAsync();
        return View(businesses);
    }

    [HttpPost("crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(string name, string industry)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RedirectToRoute(new { controller = "Negocio", action = "Index" });
        }

        var business = await _businessService.CreateAsync(name.Trim(), industry?.Trim() ?? string.Empty);
        SetActiveBusiness(business.Id);
        return Redirect("/negocio/dashboard");
    }

    [HttpPost("renombrar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Renombrar(Guid businessId, string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            await _businessService.RenameAsync(businessId, name.Trim());
        }

        return RedirectToRoute(new { controller = "Negocio", action = "Index" });
    }

    [HttpPost("seleccionar")]
    [ValidateAntiForgeryToken]
    public IActionResult Seleccionar(Guid businessId)
    {
        SetActiveBusiness(businessId);
        return Redirect("/negocio/dashboard");
    }

    private void SetActiveBusiness(Guid businessId)
    {
        Response.Cookies.Append(CookieCurrentBusinessService.CookieName, businessId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(365)
        });
    }
}
