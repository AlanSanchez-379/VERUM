using Microsoft.AspNetCore.Mvc;
using Verum.Application.Interfaces;
using Verum.Web.Models.ViewModels;

namespace Verum.Web.Controllers.Negocio;

[Route("negocio/[controller]")]
public class RentabilidadController : NegocioBaseController
{
    private readonly ISaleService _saleService;
    private readonly IBusinessExpenseService _expenseService;

    public RentabilidadController(
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

        var vm = new NegocioRentabilidadViewModel
        {
            Business = business,
            TotalSales = totalSales,
            TotalExpenses = totalExpenses,
            Utility = totalSales - totalExpenses
        };

        return View(vm);
    }
}
