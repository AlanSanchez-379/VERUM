using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseInvestmentRepository : IInvestmentRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseInvestmentRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Investment>> GetAllAsync(Guid businessId)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<InvestmentRow>()
            .Where(i => i.UserId == userId)
            .Where(i => i.BusinessId == businessId)
            .Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task AddAsync(Investment investment)
    {
        var row = new InvestmentRow
        {
            UserId = _currentUser.UserId,
            BusinessId = investment.BusinessId,
            Description = investment.Description,
            Amount = investment.Amount,
            Date = investment.Date
        };

        var response = await _client.From<InvestmentRow>().Insert(row);
        investment.Id = response.Models.First().Id;
    }

    private static Investment ToEntity(InvestmentRow row) => new()
    {
        Id = row.Id,
        BusinessId = row.BusinessId,
        Description = row.Description,
        Amount = row.Amount,
        Date = row.Date
    };
}
