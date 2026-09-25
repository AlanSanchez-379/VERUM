using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioExpensesViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<BusinessExpenseDto> Expenses { get; set; } = new();
    public decimal Total => Expenses.Sum(e => e.Amount);
}
