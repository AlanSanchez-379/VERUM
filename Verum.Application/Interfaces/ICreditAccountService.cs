using Verum.Application.DTOs.Personal;

namespace Verum.Application.Interfaces;

public interface ICreditAccountService
{
    Task<List<CreditAccountDto>> GetAllAsync();
}
