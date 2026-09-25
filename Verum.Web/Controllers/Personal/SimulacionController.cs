using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class SimulacionController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IIncomeService _incomeService;
    private readonly ICommitmentService _commitmentService;
    private readonly IExpenseService _expenseService;
    private readonly IGoalService _goalService;

    public SimulacionController(
        IAccountService accountService,
        IIncomeService incomeService,
        ICommitmentService commitmentService,
        IExpenseService expenseService,
        IGoalService goalService)
    {
        _accountService = accountService;
        _incomeService = incomeService;
        _commitmentService = commitmentService;
        _expenseService = expenseService;
        _goalService = goalService;
    }

    public async Task<IActionResult> Index()
    {
        var totalAvailable = await _accountService.GetTotalAvailableAsync();
        var incomeReceived = await _incomeService.GetTotalReceivedAsync();
        var commitmentsTotal = await _commitmentService.GetTotalAsync();
        var expensesTotal = await _expenseService.GetTotalAsync();
        var margin = Math.Max(1, incomeReceived - commitmentsTotal - expensesTotal);
        var goal = await _goalService.GetPriorityGoalAsync();

        var vm = new SimulacionViewModel
        {
            TotalAvailable = totalAvailable,
            Margin = margin,
            Goal = goal
        };

        return View(vm);
    }
}
