using Verum.Application.DTOs.Personal;

namespace Verum.Web.Models.ViewModels;

public class RecurringIncomesViewModel
{
    public List<RecurringIncomeDto> Patterns { get; set; } = new();
    public List<PendingIncomeConfirmationDto> Pending { get; set; } = new();
}
