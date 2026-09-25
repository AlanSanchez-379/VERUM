using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioRentabilidadViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public decimal TotalSales { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Utility { get; set; }

    public int MarginPercent => TotalSales <= 0 ? 0 : (int)Math.Round(Math.Max(0, Utility) / TotalSales * 100m);
    public int CostRatioPercent => TotalSales <= 0 ? 0 : (int)Math.Round(Math.Min(TotalExpenses, TotalSales) / TotalSales * 100m);
}
