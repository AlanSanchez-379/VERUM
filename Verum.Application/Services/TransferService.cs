using Verum.Application.DTOs.Negocio;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;

namespace Verum.Application.Services;

public class TransferService : ITransferService
{
    private readonly ITransferRepository _transferRepository;
    private readonly IAccountRepository _personalAccountRepository;
    private readonly IBusinessAccountRepository _businessAccountRepository;

    public TransferService(
        ITransferRepository transferRepository,
        IAccountRepository personalAccountRepository,
        IBusinessAccountRepository businessAccountRepository)
    {
        _transferRepository = transferRepository;
        _personalAccountRepository = personalAccountRepository;
        _businessAccountRepository = businessAccountRepository;
    }

    public async Task<List<TransferDto>> GetRecentAsync(Guid businessId, int take = 10)
    {
        var transfers = await _transferRepository.GetRecentAsync(businessId, take);
        if (transfers.Count == 0)
        {
            return new List<TransferDto>();
        }

        var personalAccounts = await _personalAccountRepository.GetAllAsync();
        var businessAccounts = await _businessAccountRepository.GetAllAsync(businessId);

        return transfers.Select(t => ToDto(t, personalAccounts, businessAccounts)).ToList();
    }

    public async Task<TransferResult> TransferAsync(Guid businessId, Guid personalAccountId, Guid businessAccountId, TransferDirection direction, decimal amount, string note)
    {
        if (amount <= 0)
        {
            return new TransferResult(false, "El monto debe ser mayor a cero.");
        }

        var personalAccount = await _personalAccountRepository.GetByIdAsync(personalAccountId);
        if (personalAccount is null)
        {
            return new TransferResult(false, "La cuenta personal no existe.");
        }

        var businessAccount = await _businessAccountRepository.GetByIdAsync(businessId, businessAccountId);
        if (businessAccount is null)
        {
            return new TransferResult(false, "La cuenta del negocio no existe.");
        }

        if (direction == TransferDirection.ToBusiness)
        {
            if (personalAccount.Balance < amount)
            {
                return new TransferResult(false, "No tenés ese monto disponible en esa cuenta personal.");
            }

            await _personalAccountRepository.UpdateBalanceAsync(personalAccountId, personalAccount.Balance - amount);
            await _businessAccountRepository.UpdateBalanceAsync(businessId, businessAccountId, businessAccount.Balance + amount);
        }
        else
        {
            if (businessAccount.Balance < amount)
            {
                return new TransferResult(false, "El negocio no tiene ese monto disponible en esa cuenta.");
            }

            await _businessAccountRepository.UpdateBalanceAsync(businessId, businessAccountId, businessAccount.Balance - amount);
            await _personalAccountRepository.UpdateBalanceAsync(personalAccountId, personalAccount.Balance + amount);
        }

        await _transferRepository.AddAsync(new Transfer
        {
            BusinessId = businessId,
            PersonalAccountId = personalAccountId,
            BusinessAccountId = businessAccountId,
            Direction = direction,
            Amount = amount,
            Note = note,
            Date = DateTime.UtcNow
        });

        return new TransferResult(true, null);
    }

    private static TransferDto ToDto(Transfer t, List<Account> personalAccounts, List<BusinessAccount> businessAccounts)
    {
        var personalName = personalAccounts.FirstOrDefault(a => a.Id == t.PersonalAccountId)?.Name ?? "Cuenta personal";
        var businessName = businessAccounts.FirstOrDefault(a => a.Id == t.BusinessAccountId)?.Name ?? "Cuenta del negocio";
        return new TransferDto(
            t.Id,
            t.BusinessId,
            t.PersonalAccountId,
            personalName,
            t.BusinessAccountId,
            businessName,
            t.Direction == TransferDirection.ToBusiness ? "to_business" : "to_personal",
            t.Amount,
            t.Note,
            t.Date);
    }
}
