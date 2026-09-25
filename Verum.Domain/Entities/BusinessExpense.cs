namespace Verum.Domain.Entities;

public class BusinessExpense
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
