namespace Verum.Domain.Entities;

public class Debt
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal MonthlyPayment { get; set; }
    public DateTime NextDueDate { get; set; }
}
