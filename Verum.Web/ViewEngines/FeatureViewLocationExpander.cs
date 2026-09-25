using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Verum.Web.ViewEngines;

// Permite que los controllers bajo Controllers/Personal y Controllers/Negocio
// resuelvan sus vistas en Views/Personal/{Controller}/{Action}.cshtml y
// Views/Negocio/{Controller}/{Action}.cshtml respectivamente, evitando choques
// entre controllers con el mismo nombre en distinto namespace (ej. DineroController,
// DashboardController). PopulateValues agrega "feature" a la clave de cache de
// Razor: sin esto, dos controllers con igual nombre pero distinto namespace
// comparten la misma entrada de cache y una resuelve la vista de la otra.
public class FeatureViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
        context.Values["feature"] = GetFeature(context);
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        var feature = GetFeature(context);

        if (feature is null)
        {
            return viewLocations;
        }

        var featureLocations = new[]
        {
            $"/Views/{feature}/{{1}}/{{0}}.cshtml"
        };

        return featureLocations.Concat(viewLocations);
    }

    private static string? GetFeature(ViewLocationExpanderContext context)
    {
        var ns = (context.ActionContext.ActionDescriptor as ControllerActionDescriptor)?.ControllerTypeInfo.Namespace ?? string.Empty;

        return ns.EndsWith(".Personal") ? "Personal"
            : ns.EndsWith(".Negocio") ? "Negocio"
            : null;
    }
}
