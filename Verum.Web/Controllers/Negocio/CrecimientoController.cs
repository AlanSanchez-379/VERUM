using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class CrecimientoController : NegocioBaseController
{
    private readonly ISaleService _saleService;

    public CrecimientoController(IBusinessService businessService, ICurrentBusinessService currentBusiness, ISaleService saleService)
        : base(businessService, currentBusiness)
    {
        _saleService = saleService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var sales = await _saleService.GetAllAsync(business.Id);

        var byMonth = sales
            .GroupBy(s => new DateTime(s.Date.Year, s.Date.Month, 1))
            .OrderBy(g => g.Key)
            .TakeLast(6)
            .Select(g => new { Month = g.Key, Total = g.Sum(s => s.Amount) })
            .ToList();

        var maxAmount = byMonth.Count == 0 ? 0 : byMonth.Max(m => m.Total);

        var months = byMonth.Select(m => new MonthlySalesEntry
        {
            Label = m.Month.ToString("MMM yyyy", CultureInfo.InvariantCulture),
            Amount = m.Total,
            BarPercent = maxAmount <= 0 ? 0 : (int)Math.Round(m.Total / maxAmount * 100m)
        }).ToList();

        decimal? growthPercent = null;
        decimal currentMonthSales = 0;
        decimal previousMonthSales = 0;

        if (byMonth.Count >= 2)
        {
            currentMonthSales = byMonth[^1].Total;
            previousMonthSales = byMonth[^2].Total;
            growthPercent = previousMonthSales <= 0
                ? null
                : Math.Round((currentMonthSales - previousMonthSales) / previousMonthSales * 100m, 1);
        }
        else if (byMonth.Count == 1)
        {
            currentMonthSales = byMonth[0].Total;
        }

        var vm = new NegocioCrecimientoViewModel
        {
            Business = business,
            Months = months,
            GrowthPercent = growthPercent,
            CurrentMonthSales = currentMonthSales,
            PreviousMonthSales = previousMonthSales
        };

        return View(vm);
    }
}
