namespace Verum.Web.Models.ViewModels;

public class LedgerEntry
{
    public string Label { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty; // Ingreso, Gasto, Compromiso
    public decimal Amount { get; set; } // positivo = entra, negativo = sale
    public DateTime Date { get; set; }
    public bool IsPending { get; set; }
}

public class DineroViewModel
{
    public decimal TotalAvailable { get; set; }
    public decimal Margin { get; set; }
    public List<LedgerEntry> Entries { get; set; } = new();
}
