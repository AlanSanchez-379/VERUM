namespace Verum.Domain.Entities;

public enum TransferDirection
{
    ToBusiness,
    ToPersonal
}

public class Transfer
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid PersonalAccountId { get; set; }
    public Guid BusinessAccountId { get; set; }
    public TransferDirection Direction { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
