namespace Verum.Domain.Entities;

public class Income
{
    public Guid Id { get; set; }
    public string Source { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpectedDate { get; set; }
    public bool IsReceived { get; set; }
}
