using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IBusinessExpenseRepository
{
    Task<List<BusinessExpense>> GetAllAsync(Guid businessId);
    Task<List<BusinessExpense>> GetRecentAsync(Guid businessId, int count);
    Task AddAsync(BusinessExpense expense);
}
