using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IIncomeService _incomeService;
    private readonly ICommitmentService _commitmentService;
    private readonly IExpenseService _expenseService;
    private readonly IGoalService _goalService;
    private readonly ICreditAccountService _creditAccountService;
    private readonly IRecurringIncomeService _recurringIncomeService;

    public DashboardController(
        IAccountService accountService,
        IIncomeService incomeService,
        ICommitmentService commitmentService,
        IExpenseService expenseService,
        IGoalService goalService,
        ICreditAccountService creditAccountService,
        IRecurringIncomeService recurringIncomeService)
    {
        _accountService = accountService;
        _incomeService = incomeService;
        _commitmentService = commitmentService;
        _expenseService = expenseService;
        _goalService = goalService;
        _creditAccountService = creditAccountService;
        _recurringIncomeService = recurringIncomeService;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountService.GetAccountsAsync();
        var incomes = await _incomeService.GetCurrentPeriodAsync();
        var commitments = await _commitmentService.GetCurrentPeriodAsync();
        var recentExpenses = await _expenseService.GetRecentAsync(4);
        var totalExpenses = await _expenseService.GetTotalAsync();
        var priorityGoal = await _goalService.GetPriorityGoalAsync();
        var creditAccounts = await _creditAccountService.GetAllAsync();
        var pendingConfirmations = await _recurringIncomeService.GetPendingConfirmationsAsync();

        var vm = new DashboardViewModel
        {
            Accounts = accounts,
            TotalAvailable = accounts.Sum(a => a.Balance),

            IncomeReceived = incomes.Where(i => i.IsReceived).Sum(i => i.Amount),
            IncomeExpected = incomes.Sum(i => i.Amount),
            IncomeReceivedCount = incomes.Count(i => i.IsReceived),
            IncomeTotalCount = incomes.Count,

            Commitments = commitments,
            CommitmentsTotal = commitments.Sum(c => c.Amount),

            RecentExpenses = recentExpenses,
            ExpensesTotal = totalExpenses,

            PriorityGoal = priorityGoal,

            CreditAvailable = creditAccounts.Sum(c => c.Available),
            PendingIncomeConfirmations = pendingConfirmations
        };

        return View(vm);
    }
}
