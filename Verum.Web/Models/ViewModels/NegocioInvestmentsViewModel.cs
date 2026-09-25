using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioInvestmentsViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<InvestmentDto> Investments { get; set; } = new();
    public decimal Total => Investments.Sum(i => i.Amount);
}
