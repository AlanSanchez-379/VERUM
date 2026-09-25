using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Verum.Infrastructure.Supabase;

namespace Verum.Web.Filters;

public class RequireSupabaseAuthFilter : IAsyncActionFilter
{
    private readonly global::Supabase.Client _client;
    private readonly SupabaseClientOptions _options;

    public RequireSupabaseAuthFilter(global::Supabase.Client client, IOptions<SupabaseClientOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var allowsAnonymous = context.ActionDescriptor.EndpointMetadata.Any(m => m is AllowAnonymousAttribute);

        if (_options.UseDummyData || allowsAnonymous)
        {
            await next();
            return;
        }

        if (_client.Auth.CurrentUser is null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        await next();
    }
}
