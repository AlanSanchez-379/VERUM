using Microsoft.Extensions.Options;
using Verum.Infrastructure.Supabase;

namespace Verum.Web.Middleware;

public class SupabaseSessionMiddleware
{
    public const string AccessTokenCookie = "sb-access-token";
    public const string RefreshTokenCookie = "sb-refresh-token";

    private readonly RequestDelegate _next;

    public SupabaseSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, global::Supabase.Client client, IOptions<SupabaseClientOptions> options)
    {
        if (!options.Value.UseDummyData)
        {
            var accessToken = context.Request.Cookies[AccessTokenCookie];
            var refreshToken = context.Request.Cookies[RefreshTokenCookie];

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    await client.Auth.SetSession(accessToken, refreshToken);
                }
                catch
                {
                    context.Response.Cookies.Delete(AccessTokenCookie);
                    context.Response.Cookies.Delete(RefreshTokenCookie);
                }
            }
        }

        await _next(context);
    }
}
