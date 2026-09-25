namespace Verum.Domain.Entities;

public class CreditAccount
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
    public decimal UsedAmount { get; set; }
    public DateTime CutoffDate { get; set; }
    public DateTime DueDate { get; set; }
}
