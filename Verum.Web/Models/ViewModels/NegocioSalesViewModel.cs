using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioSalesViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<SaleDto> Sales { get; set; } = new();
    public List<BusinessAccountDto> Accounts { get; set; } = new();
    public decimal Total => Sales.Sum(s => s.Amount);
}
