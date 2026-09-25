using Verum.Application.DTOs.Personal;

namespace Verum.Web.Models.ViewModels;

public class SimulacionViewModel
{
    public decimal TotalAvailable { get; set; }
    public decimal Margin { get; set; }
    public GoalDto? Goal { get; set; }
}
