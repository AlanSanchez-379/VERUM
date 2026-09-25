namespace Verum.Application.DTOs.Personal;

public record CreditAccountDto(Guid Id, string Name, decimal CreditLimit, decimal UsedAmount, DateTime CutoffDate, DateTime DueDate)
{
    public decimal Available => Math.Max(0, CreditLimit - UsedAmount);
    public int PercentUsed => CreditLimit <= 0
        ? 0
        : (int)Math.Round(Math.Min(100m, UsedAmount / CreditLimit * 100m));
}
