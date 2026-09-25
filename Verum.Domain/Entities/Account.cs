using Verum.Domain.Enums;

namespace Verum.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public string Subtitle { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
