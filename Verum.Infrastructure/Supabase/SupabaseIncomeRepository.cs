using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseIncomeRepository : IIncomeRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseIncomeRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Income>> GetAllForCurrentPeriodAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<IncomeRow>().Where(i => i.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Income income)
    {
        var row = new IncomeRow
        {
            UserId = _currentUser.UserId,
            AccountId = income.AccountId,
            Source = income.Source,
            Amount = income.Amount,
            ExpectedDate = income.ExpectedDate,
            IsReceived = income.IsReceived
        };

        var response = await _client.From<IncomeRow>().Insert(row);
        income.Id = response.Models.First().Id;
    }

    private static Income ToEntity(IncomeRow row) => new()
    {
        Id = row.Id,
        AccountId = row.AccountId,
        Source = row.Source,
        Amount = row.Amount,
        ExpectedDate = row.ExpectedDate,
        IsReceived = row.IsReceived
    };
}
