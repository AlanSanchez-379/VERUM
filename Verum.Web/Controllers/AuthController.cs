using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verum.Web.Middleware;

namespace Verum.Web.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly global::Supabase.Client _client;

    public AuthController(global::Supabase.Client client)
    {
        _client = client;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (_client.Auth.CurrentUser is not null)
        {
            return Redirect("/Dashboard");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignInWithPassword(email, password);
            if (session?.AccessToken is null)
            {
                ViewData["Error"] = "Correo o contraseña incorrectos.";
                return View();
            }

            SetSessionCookies(session.AccessToken, session.RefreshToken!);
            return Redirect("/Dashboard");
        }
        catch (Exception)
        {
            ViewData["Error"] = "Correo o contraseña incorrectos.";
            return View();
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (_client.Auth.CurrentUser is not null)
        {
            return Redirect("/Dashboard");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignUp(email, password);
            if (session?.AccessToken is not null)
            {
                SetSessionCookies(session.AccessToken, session.RefreshToken!);
                return Redirect("/Dashboard");
            }

            ViewData["Info"] = "Cuenta creada. Revisa tu correo para confirmarla y luego inicia sesión.";
            return View();
        }
        catch (Exception ex)
        {
            ViewData["Error"] = ex.Message;
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _client.Auth.SignOut();
        }
        catch
        {
            // sesión ya invalida del lado del servidor
        }

        Response.Cookies.Delete(SupabaseSessionMiddleware.AccessTokenCookie);
        Response.Cookies.Delete(SupabaseSessionMiddleware.RefreshTokenCookie);
        return RedirectToAction(nameof(Login));
    }

    private void SetSessionCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        };

        Response.Cookies.Append(SupabaseSessionMiddleware.AccessTokenCookie, accessToken, cookieOptions);
        Response.Cookies.Append(SupabaseSessionMiddleware.RefreshTokenCookie, refreshToken, cookieOptions);
    }
}
