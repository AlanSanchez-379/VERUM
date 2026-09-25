namespace Verum.Domain.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
