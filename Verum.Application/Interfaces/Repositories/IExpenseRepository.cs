using Verum.Domain.Entities;

namespace Verum.Application.Interfaces.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetRecentAsync(int count);
    Task<List<Expense>> GetAllAsync();
    Task AddAsync(Expense expense);
}
