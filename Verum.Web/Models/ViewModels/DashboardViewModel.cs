using Verum.Application.DTOs.Personal;

namespace Verum.Web.Models.ViewModels;

public class DashboardViewModel
{
    public List<AccountDto> Accounts { get; set; } = new();
    public decimal TotalAvailable { get; set; }

    public decimal IncomeReceived { get; set; }
    public decimal IncomeExpected { get; set; }
    public int IncomeReceivedCount { get; set; }
    public int IncomeTotalCount { get; set; }

    public List<CommitmentDto> Commitments { get; set; } = new();
    public decimal CommitmentsTotal { get; set; }

    public List<ExpenseDto> RecentExpenses { get; set; } = new();
    public decimal ExpensesTotal { get; set; }

    public GoalDto? PriorityGoal { get; set; }
    public decimal CreditAvailable { get; set; }

    public decimal Margin => Math.Max(0, IncomeReceived - CommitmentsTotal - ExpensesTotal);

    public int CommittedPercent => IncomeReceived <= 0 ? 0 : (int)Math.Round(Math.Min(100m, CommitmentsTotal / IncomeReceived * 100m));
    public int SpentPercent => IncomeReceived <= 0 ? 0 : (int)Math.Round(Math.Min(100m, ExpensesTotal / IncomeReceived * 100m));
    public int MarginPercent => Math.Max(0, 100 - CommittedPercent - SpentPercent);
}
