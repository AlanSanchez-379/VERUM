namespace Verum.Application.DTOs.Personal;

public record DebtDto(Guid Id, string Name, decimal TotalAmount, decimal RemainingAmount, decimal MonthlyPayment, DateTime NextDueDate)
{
    public int PercentPaid => TotalAmount <= 0
        ? 0
        : (int)Math.Round(Math.Min(100m, (TotalAmount - RemainingAmount) / TotalAmount * 100m));
}
