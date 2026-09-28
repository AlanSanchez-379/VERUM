using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseBusinessExpenseRepository : IBusinessExpenseRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseBusinessExpenseRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<BusinessExpense>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessExpenseRow>()
            .Where(e => e.UserId == userId)
            .Where(e => e.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<List<BusinessExpense>> GetRecentAsync(Guid businessId, int count)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessExpenseRow>()
            .Where(e => e.UserId == userId)
            .Where(e => e.BusinessId == businessId)
            .Order(e => e.Date, Postgrest.Constants.Ordering.Descending)
            .Limit(count)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(BusinessExpense expense)
    {
        var row = new BusinessExpenseRow
        {
            UserId = _currentUser.UserId,
            BusinessId = expense.BusinessId,
            AccountId = expense.AccountId,
            Category = expense.Category,
            Amount = expense.Amount,
            Date = expense.Date
        };

        var response = await _client.From<BusinessExpenseRow>().Insert(row);
        expense.Id = response.Models.First().Id;
    }

    private static BusinessExpense ToEntity(BusinessExpenseRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        AccountId = row.AccountId,
        Category = row.Category,
        Amount = row.Amount,
        Date = row.Date
    };
}
