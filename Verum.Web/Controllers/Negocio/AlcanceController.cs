using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class AlcanceController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessExpenseService _expenseService;
    private readonly IBusinessGoalService _goalService;

    public AlcanceController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        ISaleService saleService,
        IBusinessExpenseService expenseService,
        IBusinessGoalService goalService)
        : base(businessService, currentBusiness)
    {
        _saleService = saleService;
        _expenseService = expenseService;
        _goalService = goalService;
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
        var margin = Math.Max(0, totalSales - totalExpenses);

        var reach = await _goalService.GetReachAnalysisAsync(business.Id, margin);

        var vm = new NegocioAlcanceViewModel
        {
            Business = business,
            Margin = margin,
            Reach = reach
        };

        return View(vm);
    }
}
