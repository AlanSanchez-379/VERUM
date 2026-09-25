using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioSimulacionViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public decimal Available { get; set; }
    public decimal Margin { get; set; }
    public BusinessGoalDto? Goal { get; set; }
}
