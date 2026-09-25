using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class AnalisisController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessExpenseService _expenseService;

    public AnalisisController(
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
        var expenses = await _expenseService.GetAllAsync(business.Id);
        var totalExpenses = expenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategoryBreakdown
            {
                Category = g.Key,
                Amount = g.Sum(e => e.Amount),
                Percent = totalExpenses <= 0 ? 0 : (int)Math.Round(g.Sum(e => e.Amount) / totalExpenses * 100m)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        var vm = new NegocioAnalisisViewModel
        {
            Business = business,
            TotalSales = totalSales,
            TotalExpenses = totalExpenses,
            Utility = totalSales - totalExpenses,
            ExpensesByCategory = byCategory
        };

        return View(vm);
    }
}
