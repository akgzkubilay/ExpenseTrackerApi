using ExpenseTrackerApi.Models;
using ExpenseTrackerApi.Repositories; // Veri katmanımızı tanıttık
using ExpenseTrackerApi.Services;     // İş katmanımızı tanıttık

var builder = WebApplication.CreateBuilder(args);

// 1. CORS Ayarları
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// 2. DEPENDENCY INJECTION (BAĞIMLILIK ENJEKSİYONU) KABLO BAĞLANTILARI
// Sisteme diyoruz ki: "Biri senden interface isterse, ona asıl işi yapan class'ı ver."
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

var app = builder.Build();

// 3. ARA KATMANLAR (MIDDLEWARE)
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

// --- 4. TEMİZLENMİŞ ENDPOINT'LER (API YOLLARI) ---

// Harcama Ekleme (POST) - Artık SQL yok, sadece Service ile konuşuyor!
app.MapPost("/api/expenses", async (Expense expense, IExpenseService service) =>
{
    var result = await service.AddExpenseAsync(expense);
    
    if (result == null) 
        return Results.BadRequest("Hata: Tutar 0'dan büyük olmalı ve kategori boş olamaz.");
        
    return Results.Created($"/api/expenses/{result.Id}", result);
});

// Harcamaları Listeleme (GET)
app.MapGet("/api/expenses", async (IExpenseService service) =>
{
    var expenses = await service.GetAllExpensesAsync();
    return Results.Ok(expenses);
});

// Harcama Silme (DELETE)
app.MapDelete("/api/expenses/{id}", async (int id, IExpenseService service) =>
{
    var isDeleted = await service.DeleteExpenseAsync(id);
    
    if (isDeleted == null) 
        return Results.NotFound("Silinecek harcama bulunamadı.");
        
    return Results.Ok("Harcama başarıyla silindi.");
});

app.Run();