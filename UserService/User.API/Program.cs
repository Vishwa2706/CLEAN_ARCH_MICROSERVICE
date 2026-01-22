using Microsoft.EntityFrameworkCore;
using User.Application.Contracts;
using User.Application.Query;
using User.Infrastructure.Repository;
using User.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<UserRepository>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<GetAllUserQuery>();
builder.Services.AddScoped<GetUserExpensesQuery>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
