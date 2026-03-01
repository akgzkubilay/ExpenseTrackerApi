using ExpenseTrackerApi.Models;
using ExpenseTrackerApi.Repositories;

namespace ExpenseTrackerApi.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
public async Task<Expense?> AddExpenseAsync(Expense expense)
    {
        // İŞ KURALI 1: Tutar 0 veya negatif olamaz!
        if (expense.Amount <= 0)
        {
            return null; 
        }

        // İŞ KURALI 2: Kategori boş gönderilemez!
        if (string.IsNullOrWhiteSpace(expense.Category))
        {
            return null;
        }

        // YENİ EKLENEN KURAL: Eğer tarih atanmamışsa, şu anki zamanı otomatik ata!
        if (expense.Date == default)
        {
            expense.Date = DateTime.Now;
        }

        // Kuralları geçtiyse, veritabanına kaydetmesi için Repository'ye gönder
        int newId = await _expenseRepository.AddAsync(expense);
        expense.Id = newId;
        
        return expense;
    }

        public async Task<Expense?> DeleteExpenseAsync(int id)
        {
            return await _expenseRepository.DeleteAsync(id).ContinueWith(task =>
            {
                if (task.Result)
                {
                    return new Expense { Id = id };
                }
                else
                {
                    throw new KeyNotFoundException($"Expense with id {id} not found.");
                }
            });
        }

        public Task<IEnumerable<Expense>> GetAllExpensesAsync()
        { 
            return _expenseRepository.GetAllAsync();
        }

        public async Task<Expense> UpdateExpenseAsync(int id, Expense expense)
        {
            // İŞ KURALI 1: Tutar 0 veya negatif olamaz!
            if (expense.Amount <= 0)
            {
                return null;
            }

            // İŞ KURALI 2: Kategori boş gönderilemez!
            if (string.IsNullOrWhiteSpace(expense.Category))
            {
                return null;
            }

            bool updated = await _expenseRepository.UpdateAsync(id, expense);

            if (!updated)
            {
                throw new KeyNotFoundException($"Expense with id {id} not found.");
            }

            expense.Id = id;
            return expense;
        }
    }
}