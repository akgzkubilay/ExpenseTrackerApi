using Dapper;
using ExpenseTrackerApi.Repositories;
using ExpenseTrackerApi.Models;
using Microsoft.Data.SqlClient;


namespace ExpenseTrackerApi.Repositories
{
    public class ExpenseRepository: IExpenseRepository
    {
        private readonly string  _connectionString;
        public ExpenseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public ExpenseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> AddAsync(Expense expense)
        {
            var sql = @"INSERT INTO Expenses (Description, Amount, Category, Date) 
                VALUES (@Description, @Amount, @Category, @Date);
                SELECT CAST(SCOPE_IDENTITY() AS INT)";

            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(sql, expense);
        }


        public Task<bool> DeleteAsync(int id)
        {
           using var connection = new SqlConnection(_connectionString);
            var sql = "DELETE FROM Expenses WHERE Id = @Id";
            var affectedRows = connection.Execute(sql, new { Id = id });
            return Task.FromResult(affectedRows > 0);
        }

        public async Task<IEnumerable<Expense>> GetAllAsync()
        { 
            using var connection = new SqlConnection(_connectionString);
            var sql = "SELECT * FROM Expenses ORDER BY Date DESC";
            return await connection.QueryAsync<Expense>(sql);
            
        }

        public async Task<bool> UpdateAsync(int id, Expense expense)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "UPDATE Expenses SET Description = @Description, Amount = @Amount, Date = @Date WHERE Id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, expense.Description, expense.Amount, expense.Date });
            return affectedRows > 0;
        }
    }
}