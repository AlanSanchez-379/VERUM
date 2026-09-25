namespace Verum.Web.Models.ViewModels;

public class CategoryBreakdown
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Percent { get; set; }
}

public class AnalisisViewModel
{
    public decimal IncomeReceived { get; set; }
    public decimal CommitmentsTotal { get; set; }
    public decimal ExpensesTotal { get; set; }
    public decimal Margin { get; set; }
    public List<CategoryBreakdown> ByCategory { get; set; } = new();
}
