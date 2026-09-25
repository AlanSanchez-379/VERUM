using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class DashboardController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessExpenseService _expenseService;

    public DashboardController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        ISaleService saleService,
        IBusinessExpenseService expenseService)
        : base(businessService, currentBusiness)
    {
        _saleService = saleService;
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var totalSales = await _saleService.GetTotalAsync(business.Id);
        var totalExpenses = await _expenseService.GetTotalAsync(business.Id);
        var recentSales = await _saleService.GetRecentAsync(business.Id, 5);
        var recentExpenses = await _expenseService.GetRecentAsync(business.Id, 5);

        var vm = new NegocioDashboardViewModel
        {
            Business = business,
            TotalSales = totalSales,
            TotalExpenses = totalExpenses,
            Utility = totalSales - totalExpenses,
            RecentSales = recentSales,
            RecentExpenses = recentExpenses
        };

        return View(vm);
    }
}
