using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseRecurringIncomeRepository : IRecurringIncomeRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseRecurringIncomeRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<RecurringIncome>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<RecurringIncomeRow>().Where(r => r.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(RecurringIncome recurringIncome)
    {
        var row = new RecurringIncomeRow
        {
            UserId = _currentUser.UserId,
            Source = recurringIncome.Source,
            ExpectedAmount = recurringIncome.ExpectedAmount,
            DayOfMonth = recurringIncome.DayOfMonth,
            IsActive = recurringIncome.IsActive
        };

        var response = await _client.From<RecurringIncomeRow>().Insert(row);
        recurringIncome.Id = response.Models.First().Id;
    }

    public async Task<List<RecurringIncomeConfirmation>> GetConfirmationsForPeriodAsync(DateTime period)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<RecurringIncomeConfirmationRow>()
            .Where(c => c.UserId == userId)
            .Where(c => c.Period == period)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddConfirmationAsync(RecurringIncomeConfirmation confirmation)
    {
        var row = new RecurringIncomeConfirmationRow
        {
            UserId = _currentUser.UserId,
            RecurringIncomeId = confirmation.RecurringIncomeId,
            Period = confirmation.Period,
            ConfirmedAmount = confirmation.ConfirmedAmount,
            IncomeId = confirmation.IncomeId,
            ConfirmedAt = confirmation.ConfirmedAt
        };

        await _client.From<RecurringIncomeConfirmationRow>().Insert(row);
    }

    private static RecurringIncome ToEntity(RecurringIncomeRow row) => new()
    {
        Id = row.Id,
        Source = row.Source,
        ExpectedAmount = row.ExpectedAmount,
        DayOfMonth = row.DayOfMonth,
        IsActive = row.IsActive
    };

    private static RecurringIncomeConfirmation ToEntity(RecurringIncomeConfirmationRow row) => new()
    {
        Id = row.Id,
        RecurringIncomeId = row.RecurringIncomeId,
        Period = row.Period,
        ConfirmedAmount = row.ConfirmedAmount,
        IncomeId = row.IncomeId,
        ConfirmedAt = row.ConfirmedAt
    };
}
