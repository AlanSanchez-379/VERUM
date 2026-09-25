using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioReceivablesViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<ReceivableDto> Receivables { get; set; } = new();
    public decimal Total => Receivables.Sum(r => r.Amount);
    public decimal Pending => Receivables.Where(r => !r.IsCollected).Sum(r => r.Amount);
}
