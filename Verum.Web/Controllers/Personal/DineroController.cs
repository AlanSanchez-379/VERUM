using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Personal;

[Route("personal/[controller]")]
public class DineroController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IIncomeService _incomeService;
    private readonly ICommitmentService _commitmentService;
    private readonly IExpenseService _expenseService;

    public DineroController(
        IAccountService accountService,
        IIncomeService incomeService,
        ICommitmentService commitmentService,
        IExpenseService expenseService)
    {
        _accountService = accountService;
        _incomeService = incomeService;
        _commitmentService = commitmentService;
        _expenseService = expenseService;
    }

    public async Task<IActionResult> Index()
    {
        var totalAvailable = await _accountService.GetTotalAvailableAsync();
        var incomes = await _incomeService.GetCurrentPeriodAsync();
        var commitments = await _commitmentService.GetCurrentPeriodAsync();
        var expenses = await _expenseService.GetAllAsync();

        var entries = new List<LedgerEntry>();

        entries.AddRange(incomes.Select(i => new LedgerEntry
        {
            Label = i.Source,
            Kind = "Ingreso",
            Amount = i.Amount,
            Date = i.ExpectedDate,
            IsPending = !i.IsReceived
        }));

        entries.AddRange(commitments.Select(c => new LedgerEntry
        {
            Label = c.Name,
            Kind = "Compromiso",
            Amount = -c.Amount,
            Date = c.DueDate,
            IsPending = !c.IsPaid
        }));

        entries.AddRange(expenses.Select(e => new LedgerEntry
        {
            Label = e.Category,
            Kind = "Gasto",
            Amount = -e.Amount,
            Date = e.Date,
            IsPending = false
        }));

        var margin = Math.Max(0,
            incomes.Where(i => i.IsReceived).Sum(i => i.Amount)
            - commitments.Sum(c => c.Amount)
            - expenses.Sum(e => e.Amount));

        var vm = new DineroViewModel
        {
            TotalAvailable = totalAvailable,
            Margin = margin,
            Entries = entries.OrderByDescending(e => e.Date).ToList()
        };

        return View(vm);
    }
}
