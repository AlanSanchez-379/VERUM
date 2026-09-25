using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioDashboardViewModel
{
    public BusinessDto Business { get; set; } = null!;

    public decimal TotalSales { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Utility { get; set; }

    public List<SaleDto> RecentSales { get; set; } = new();
    public List<BusinessExpenseDto> RecentExpenses { get; set; } = new();
}
