using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseDebtRepository : IDebtRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseDebtRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Debt>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<DebtRow>().Where(d => d.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    private static Debt ToEntity(DebtRow row) => new()
    {
        Id = row.Id,
        Name = row.Name,
        TotalAmount = row.TotalAmount,
        RemainingAmount = row.RemainingAmount,
        MonthlyPayment = row.MonthlyPayment,
        NextDueDate = row.NextDueDate
    };
}
