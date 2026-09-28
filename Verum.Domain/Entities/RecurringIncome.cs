namespace Verum.Domain.Entities;

public class RecurringIncome
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public decimal ExpectedAmount { get; set; }
    public int DayOfMonth { get; set; }
    public bool IsActive { get; set; } = true;
}
