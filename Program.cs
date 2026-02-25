using Microsoft.Data.SqlClient;
using Dapper;
using ExprenseTrackerApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. AŞAMA: MALZEMELERİ EKLİYORUZ (Build'den ÖNCE olmalı!)
// CORS yetkisini sisteme tanıtıyoruz
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// --- KEKİ FIRINA VERİYORUZ (Artık builder.Services kullanılamaz) ---
var app = builder.Build();

// 2. AŞAMA: ARA KATMANLARI (MIDDLEWARE) EKLİYORUZ
app.UseCors(); // CORS'u aktif et
app.UseDefaultFiles(); // index.html'i varsayılan sayfa yap
app.UseStaticFiles();  // wwwroot klasörünü dışarıya aç

// appsettings.json içindeki bağlantı dizesini alıyoruz
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// --- 3. AŞAMA: ENDPOINT'LER (API YOLLARI) ---

// Yeni Harcama Ekleme (POST)
app.MapPost("/api/expenses", async (Expense expense) =>
{
    // Tarih gönderilmemişse bugünün tarihini kullan
    if (expense.Date == default)
        expense.Date = DateTime.Now;

    using var connection = new SqlConnection(connectionString);
    var sql = @"
        INSERT INTO Expenses (Amount, Category, Description, Date) 
        VALUES (@Amount, @Category, @Description, @Date);
        SELECT CAST(SCOPE_IDENTITY() as int);"; 
    
    var id = await connection.QuerySingleAsync<int>(sql, expense);
    expense.Id = id;
    
    return Results.Created($"/api/expenses/{id}", expense);
});

// Tüm Harcamaları Listeleme (GET)
app.MapGet("/api/expenses", async () =>
{
    using var connection = new SqlConnection(connectionString);
    var sql = "SELECT * FROM Expenses ORDER BY Date DESC";
    var expenses = await connection.QueryAsync<Expense>(sql);
    
    return Results.Ok(expenses);
});
//harcamları silme (DELETE)
app.MapDelete("/api/expenses/{id}", async (int id) =>
{
    using var connection = new SqlConnection(connectionString); 
    //dapper ile silme işlemi
    var sql = "DELETE FROM Expenses where Id = @Id";

    var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
    if (affectedRows > 0)
        return Results.NoContent(); // Silme başarılı, içerik yok
    else
        return Results.NotFound(); // Silinecek kayıt bulunamadı

});

// --- MOTORU ÇALIŞTIR ---
app.Run();