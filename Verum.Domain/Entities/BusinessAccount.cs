namespace Verum.Domain.Entities;

public class BusinessAccount
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
