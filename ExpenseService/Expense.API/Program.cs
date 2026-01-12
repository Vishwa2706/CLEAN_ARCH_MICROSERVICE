using Expense.Application.Contracts;
using Expense.Application.Query;
using Expense.Application.Commands;
using Expense.Application.Factories;
using Expense.Infrastructure.Repository;
using Expense.Infrastructure.Exporters;
using Expense.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ExpenseRepository>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register LoggerService as singleton via interface
builder.Services.AddSingleton<ILoggerService>(LoggerService.Instance);


// DI
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<GetAllExpenseQuery>();

builder.Services.AddScoped<CreateExpenseCommand>();

builder.Services.AddScoped<IExpenseExporter, CsvExpenseExporter>();
builder.Services.AddScoped<IExpenseExporter, JsonExpenseExporter>();
builder.Services.AddScoped<ExpenseExporterFactory>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
