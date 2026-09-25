using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioPayablesViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<PayableDto> Payables { get; set; } = new();
    public decimal Total => Payables.Sum(p => p.Amount);
    public decimal Pending => Payables.Where(p => !p.IsPaid).Sum(p => p.Amount);
}
