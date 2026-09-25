using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class AnalisisController : Controller
{
    private readonly IIncomeService _incomeService;
    private readonly ICommitmentService _commitmentService;
    private readonly IExpenseService _expenseService;

    public AnalisisController(
        IIncomeService incomeService,
        ICommitmentService commitmentService,
        IExpenseService expenseService)
    {
        _incomeService = incomeService;
        _commitmentService = commitmentService;
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index()
    {
        var incomeReceived = await _incomeService.GetTotalReceivedAsync();
        var commitmentsTotal = await _commitmentService.GetTotalAsync();
        var expenses = await _expenseService.GetAllAsync();
        var expensesTotal = expenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => e.Category)
            .Select(g => new CategoryBreakdown
            {
                Category = g.Key,
                Amount = g.Sum(e => e.Amount),
                Percent = expensesTotal <= 0 ? 0 : (int)Math.Round(g.Sum(e => e.Amount) / expensesTotal * 100m)
            })
            .OrderByDescending(c => c.Amount)
            .ToList();

        var vm = new AnalisisViewModel
        {
            IncomeReceived = incomeReceived,
            CommitmentsTotal = commitmentsTotal,
            ExpensesTotal = expensesTotal,
            Margin = Math.Max(0, incomeReceived - commitmentsTotal - expensesTotal),
            ByCategory = byCategory
        };

        return View(vm);
    }
}
