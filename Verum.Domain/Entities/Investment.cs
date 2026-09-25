namespace Verum.Domain.Entities;

public class Investment
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
