using ExpenseTrackerApi.Models;

namespace ExpenseTrackerApi.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<Expense>> GetAllExpensesAsync();
        Task<Expense> AddExpenseAsync(Expense expense);
        Task<Expense> DeleteExpenseAsync(int id);
        Task<Expense?> UpdateExpenseAsync(int id, Expense expense);
        
    }
}