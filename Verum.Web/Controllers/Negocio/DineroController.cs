using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class DineroController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessExpenseService _expenseService;
    private readonly ICollectionService _collectionService;
    private readonly IPayableService _payableService;

    public DineroController(
        IBusinessService businessService,
        ICurrentBusinessService currentBusiness,
        ISaleService saleService,
        IBusinessExpenseService expenseService,
        ICollectionService collectionService,
        IPayableService payableService)
        : base(businessService, currentBusiness)
    {
        _saleService = saleService;
        _expenseService = expenseService;
        _collectionService = collectionService;
        _payableService = payableService;
    }

    public async Task<IActionResult> Index()
    {
        var business = await GetActiveBusinessAsync();
        if (business is null)
        {
            return Redirect("/negocio");
        }

        var sales = await _saleService.GetAllAsync(business.Id);
        var expenses = await _expenseService.GetAllAsync(business.Id);
        var receivables = await _collectionService.GetAllAsync(business.Id);
        var payables = await _payableService.GetAllAsync(business.Id);

        var entries = new List<LedgerEntry>();

        entries.AddRange(sales.Select(s => new LedgerEntry
        {
            Label = s.Description,
            Kind = "Venta",
            Amount = s.Amount,
            Date = s.Date,
            IsPending = false
        }));

        entries.AddRange(expenses.Select(e => new LedgerEntry
        {
            Label = e.Category,
            Kind = "Gasto",
            Amount = -e.Amount,
            Date = e.Date,
            IsPending = false
        }));

        entries.AddRange(receivables.Where(r => !r.IsCollected).Select(r => new LedgerEntry
        {
            Label = r.ClientName,
            Kind = "Por cobrar",
            Amount = r.Amount,
            Date = r.DueDate,
            IsPending = true
        }));

        entries.AddRange(payables.Where(p => !p.IsPaid).Select(p => new LedgerEntry
        {
            Label = p.SupplierName,
            Kind = "Por pagar",
            Amount = -p.Amount,
            Date = p.DueDate,
            IsPending = true
        }));

        var totalSales = sales.Sum(s => s.Amount);
        var totalExpenses = expenses.Sum(e => e.Amount);

        var vm = new NegocioDineroViewModel
        {
            Business = business,
            TotalSales = totalSales,
            TotalExpenses = totalExpenses,
            Utility = totalSales - totalExpenses,
            Entries = entries.OrderByDescending(e => e.Date).ToList()
        };

        return View(vm);
    }
}
