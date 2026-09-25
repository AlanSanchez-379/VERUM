namespace Verum.Domain.Entities;

public class Payable
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPaid { get; set; }
}
