using Verum.Application.DTOs.Negocio;
using Verum.Application.DTOs.Personal;

namespace Verum.Web.Models.ViewModels;

public class NegocioTransferenciasViewModel
{
    public BusinessDto Business { get; set; } = null!;
    public List<AccountDto> PersonalAccounts { get; set; } = new();
    public List<BusinessAccountDto> BusinessAccounts { get; set; } = new();
    public List<TransferDto> RecentTransfers { get; set; } = new();
}
