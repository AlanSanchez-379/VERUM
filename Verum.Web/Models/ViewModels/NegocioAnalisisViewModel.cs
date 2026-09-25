using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioAnalisisViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public decimal TotalSales { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Utility { get; set; }
    public List<CategoryBreakdown> ExpensesByCategory { get; set; } = new();
}
