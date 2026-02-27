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
            using var connection = new SqlConnection(_connectionString);
            var sql = "INSERT INTO Expenses (Description, Amount, Date) VALUES (@Description, @Amount, @Date); SELECT CAST(SCOPE_IDENTITY() as int)";
            return  await connection.ExecuteScalarAsync<int>(sql, expense);
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
    }
}