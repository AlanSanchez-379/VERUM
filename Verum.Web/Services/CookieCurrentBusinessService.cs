using Verum.Application.Interfaces;

namespace Verum.Web.Services;

public class CookieCurrentBusinessService : ICurrentBusinessService
{
    public const string CookieName = "negocio-activo";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieCurrentBusinessService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? BusinessId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}
