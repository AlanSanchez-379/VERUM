using Verum.Application.DTOs.Personal;

namespace Verum.Web.Models.ViewModels;

public class CompromisosViewModel
{
    public List<CommitmentDto> Commitments { get; set; } = new();
    public List<AccountDto> Accounts { get; set; } = new();
}
