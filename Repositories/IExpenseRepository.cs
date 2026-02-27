namespace ExpenseTrackerApi.Repositories;

using ExpenseTrackerApi.Models;

public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<int> AddAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
}