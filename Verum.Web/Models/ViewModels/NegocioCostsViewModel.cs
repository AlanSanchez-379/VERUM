using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioCostsViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<CostDto> Costs { get; set; } = new();
    public decimal Total => Costs.Sum(c => c.Amount);
    public decimal Pending => Costs.Where(c => !c.IsPaid).Sum(c => c.Amount);
}
