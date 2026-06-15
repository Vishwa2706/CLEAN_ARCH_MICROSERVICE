using Microsoft.EntityFrameworkCore;
using Shared.Common.Middleware;
using Shared.Logging.Infrastructure;
using Shared.Reddis.Contract;
using Shared.Reddis.Service;
using User.Application.Contracts;
using User.Application.Query;
using User.Application.Services;
using User.Infrastructure.Repository;
using User.Infrastructure.Service;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<UserRepository>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

//DI
builder.Services.AddSharedLogging(builder.Configuration, "UserService");

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<GetAllUserQuery>();
builder.Services.AddScoped<GetUserExpensesQuery>();
builder.Services.AddScoped<GetFamilyAdminService>();
builder.Services.AddScoped<GetUserByUserId>();

// Redis REST
builder.Services.AddHttpClient<IRedisCacheService, UpstashRedisCacheService>();

builder.Services.AddScoped<IRefTermRepository, RefTermRepository>();
builder.Services.AddScoped<RefTermServices>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
