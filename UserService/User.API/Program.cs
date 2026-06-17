using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Middleware;
using Shared.Logging.Infrastructure;
using Shared.Reddis.Contract;
using Shared.Reddis.Service;
using User.API.Grpc;
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

builder.WebHost.ConfigureKestrel(options =>
{
    // REST

    options.ListenLocalhost(
        5180,
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        }
    );

    // gRPC

    options.ListenLocalhost(
        5181,
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        }
    );
});

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

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.MapGrpcService<UserGrpcServiceImpl>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
