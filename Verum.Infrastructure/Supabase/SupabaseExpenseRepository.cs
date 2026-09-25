using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseExpenseRepository : IExpenseRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseExpenseRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Expense>> GetRecentAsync(int count)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<ExpenseRow>()
            .Where(e => e.UserId == userId)
            .Order(e => e.Date, Postgrest.Constants.Ordering.Descending)
            .Limit(count)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<ExpenseRow>().Where(e => e.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Expense expense)
    {
        var row = new ExpenseRow
        {
            UserId = _currentUser.UserId,
            AccountId = expense.AccountId,
            Category = expense.Category,
            Amount = expense.Amount,
            Date = expense.Date
        };

        var response = await _client.From<ExpenseRow>().Insert(row);
        var inserted = response.Models.First();
        expense.Id = inserted.Id;
    }

    private static Expense ToEntity(ExpenseRow row) => new()
    {
        Id = row.Id,
        AccountId = row.AccountId,
        Category = row.Category,
        Amount = row.Amount,
        Date = row.Date
    };
}
