namespace Verum.Domain.Entities;

public class Receivable
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCollected { get; set; }
}
