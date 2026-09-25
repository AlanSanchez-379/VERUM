using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class AlcanceController : Controller
{
    private readonly IIncomeService _incomeService;
    private readonly ICommitmentService _commitmentService;
    private readonly IExpenseService _expenseService;
    private readonly IGoalService _goalService;

    public AlcanceController(
        IIncomeService incomeService,
        ICommitmentService commitmentService,
        IExpenseService expenseService,
        IGoalService goalService)
    {
        _incomeService = incomeService;
        _commitmentService = commitmentService;
        _expenseService = expenseService;
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        var incomeReceived = await _incomeService.GetTotalReceivedAsync();
        var commitmentsTotal = await _commitmentService.GetTotalAsync();
        var expensesTotal = await _expenseService.GetTotalAsync();
        var margin = Math.Max(0, incomeReceived - commitmentsTotal - expensesTotal);

        ViewData["Margin"] = margin;

        var reach = await _goalService.GetReachAnalysisAsync(margin);
        return View(reach);
    }
}
