using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class SimulacionController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessExpenseService _expenseService;
    private readonly IBusinessGoalService _goalService;

    public SimulacionController(
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
        var utility = totalSales - totalExpenses;
        var goals = await _goalService.GetAllAsync(business.Id);

        var vm = new NegocioSimulacionViewModel
        {
            Business = business,
            Available = utility,
            Margin = Math.Max(1, utility),
            Goal = goals.FirstOrDefault()
        };

        return View(vm);
    }
}
