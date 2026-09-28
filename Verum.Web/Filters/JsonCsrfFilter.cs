using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Verum.Web.Filters;

// Los endpoints que reciben JSON via fetch() (marcar pagado, registrar venta,
// subir foto, etc.) no pasan por el [ValidateAntiForgeryToken] clasico porque
// ese solo lee el token de un campo de formulario. Este filtro cubre esos
// endpoints leyendo el token del header X-CSRF-TOKEN (ver wwwroot/js/csrf.js),
// sin tener que anotar cada action individualmente.
public class JsonCsrfFilter : IAsyncActionFilter
{
    private readonly IAntiforgery _antiforgery;

    public JsonCsrfFilter(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var isUnsafeMethod = HttpMethods.IsPost(request.Method)
            || HttpMethods.IsPut(request.Method)
            || HttpMethods.IsPatch(request.Method)
            || HttpMethods.IsDelete(request.Method);

        var alreadyProtected = context.ActionDescriptor.EndpointMetadata
            .Any(m => m is ValidateAntiForgeryTokenAttribute or AllowAnonymousAttribute);

        if (isUnsafeMethod && !alreadyProtected)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException)
            {
                context.Result = new BadRequestObjectResult(new { error = "Sesión inválida, recargá la página e intentá de nuevo." });
                return;
            }
        }

        await next();
    }
}
