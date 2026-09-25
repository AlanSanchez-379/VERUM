using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioTaxesViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<TaxDto> Taxes { get; set; } = new();
    public decimal Total => Taxes.Sum(t => t.Amount);
    public decimal Pending => Taxes.Where(t => !t.IsPaid).Sum(t => t.Amount);
}
