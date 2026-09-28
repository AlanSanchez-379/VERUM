using Verum.Application.DTOs.Negocio;

namespace Verum.Web.Models.ViewModels;

public class NegocioCuentasViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<BusinessAccountDto> Accounts { get; set; } = new();
    public decimal TotalAvailable => Accounts.Sum(a => a.Balance);
}
