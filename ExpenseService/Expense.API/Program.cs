using System.Reflection;
using Expense.Application.Commands;
using Expense.Application.Contracts;
using Expense.Application.Factories;
using Expense.Application.Query;
using Expense.Application.Strategies;
using Expense.Infrastructure.Clients;
using Expense.Infrastructure.Exporters;
using Expense.Infrastructure.Persistence.Seed;
using Expense.Infrastructure.Repository;
using Expense.Infrastructure.Service;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Authorization.Extensions;
using Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ExpenseRepository>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register LoggerService as singleton via interface
builder.Services.AddSingleton<ILoggerService>(LoggerService.Instance);

// DI
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<PatchExpenseCommand>();
builder.Services.AddScoped<DeleteExpenseCommand>();

builder.Services.AddScoped<IExpenseExporter, CsvExpenseExporter>();
builder.Services.AddScoped<IExpenseExporter, JsonExpenseExporter>();
builder.Services.AddScoped<ExpenseExporterFactory>();

builder.Services.AddScoped<MonthlyExpenseSummaryStrategy>();
builder.Services.AddScoped<CategoryExpenseSummaryStrategy>();

builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5180");
});

//Seed Data
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddPermissionAuth();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("Expense.Application"));
});

var app = builder.Build();

//Temporary DI
//Program.cs → DI scope → Seeder → DbContext → Database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
