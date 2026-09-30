using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue.Exceptions;
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
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewData["Error"] = "Ingresá tu correo y contraseña.";
            return View();
        }

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
        catch (GotrueException ex)
        {
            ViewData["Error"] = MapLoginError(ex);
            return View();
        }
        catch (Exception)
        {
            ViewData["Error"] = "No se pudo conectar. Revisá tu conexión e intentá de nuevo.";
            return View();
        }
    }

    // El mensaje nunca confirma si el correo existe o no (evita que alguien
    // use el login para saber qué correos están registrados), salvo en los
    // casos que ya son públicos de por sí, como "confirmá tu correo".
    private static string MapLoginError(GotrueException ex) => ex.Reason switch
    {
        FailureHint.Reason.UserEmailNotConfirmed => "Todavía no confirmaste tu correo. Revisá tu bandeja de entrada.",
        FailureHint.Reason.UserTooManyRequests => "Demasiados intentos. Esperá un momento y volvé a intentar.",
        FailureHint.Reason.Offline => "No se pudo conectar. Revisá tu conexión e intentá de nuevo.",
        FailureHint.Reason.UserBadEmailAddress => "Ese correo no es válido.",
        _ => "Correo o contraseña incorrectos."
    };

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
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ViewData["Error"] = "Ingresá tu correo y contraseña.";
            return View();
        }

        if (password.Length < 6)
        {
            ViewData["Error"] = "La contraseña debe tener al menos 6 caracteres.";
            return View();
        }

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
        catch (GotrueException ex)
        {
            ViewData["Error"] = MapRegisterError(ex);
            return View();
        }
        catch (Exception)
        {
            ViewData["Error"] = "No se pudo conectar. Revisá tu conexión e intentá de nuevo.";
            return View();
        }
    }

    private static string MapRegisterError(GotrueException ex) => ex.Reason switch
    {
        FailureHint.Reason.UserAlreadyRegistered => "Ya existe una cuenta con ese correo. Iniciá sesión en vez de crear una nueva.",
        FailureHint.Reason.UserBadEmailAddress => "Ese correo no es válido.",
        FailureHint.Reason.UserTooManyRequests => "Se enviaron demasiados correos en poco tiempo. Esperá unos minutos y volvé a intentar.",
        FailureHint.Reason.Offline => "No se pudo conectar. Revisá tu conexión e intentá de nuevo.",
        _ => "No se pudo crear la cuenta. Intentá de nuevo en un momento."
    };

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
