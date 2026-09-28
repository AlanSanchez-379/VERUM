using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Domain.Entities;
using Verum.Infrastructure.Supabase.Models;

namespace Verum.Infrastructure.Supabase;

public class SupabaseBusinessRepository : IBusinessRepository
{
    private readonly global::Supabase.Client _client;
    private readonly ICurrentUserService _currentUser;

    public SupabaseBusinessRepository(global::Supabase.Client client, ICurrentUserService currentUser)
    {
        _client = client;
        _currentUser = currentUser;
    }

    public async Task<List<Business>> GetAllAsync()
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessRow>().Where(b => b.UserId == userId).Get();
        return response.Models.Select(ToEntity).ToList();
    }

    public async Task<Business?> GetByIdAsync(Guid id)
    {
        var userId = _currentUser.UserId;
        var response = await _client.From<BusinessRow>()
            .Where(b => b.UserId == userId)
            .Where(b => b.Id == id)
            .Get();
        var row = response.Models.FirstOrDefault();
        return row is null ? null : ToEntity(row);
    }

    public async Task<Business> CreateAsync(Business business)
    {
        var row = new BusinessRow
        {
            UserId = _currentUser.UserId,
            Name = business.Name,
            Industry = business.Industry,
            CreatedAt = business.CreatedAt
        };

        var response = await _client.From<BusinessRow>().Insert(row);
        return ToEntity(response.Models.First());
    }

    public async Task RenameAsync(Guid id, string name)
    {
        var userId = _currentUser.UserId;
        await _client.From<BusinessRow>()
            .Where(b => b.UserId == userId)
            .Where(b => b.Id == id)
            .Set(b => b.Name, name)
            .Update();
    }

    private static Business ToEntity(BusinessRow row) => new()
    {
        Id = row.Id,
        Name = row.Name,
        Industry = row.Industry,
        CreatedAt = row.CreatedAt
    };
}
