using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class MonthlySalesEntry
{
    public string Label { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int BarPercent { get; set; }
}

public class NegocioCrecimientoViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<MonthlySalesEntry> Months { get; set; } = new();
    public decimal? GrowthPercent { get; set; }
    public decimal CurrentMonthSales { get; set; }
    public decimal PreviousMonthSales { get; set; }
}
