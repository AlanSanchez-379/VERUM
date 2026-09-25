using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioAlcanceViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public decimal Margin { get; set; }
    public List<BusinessGoalReachDto> Reach { get; set; } = new();
}
