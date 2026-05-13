using APIGateway.YARP.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Load YARP config
builder.Configuration.AddJsonFile("appsettings.yarp.json", optional: false, reloadOnChange: true);

// Register services
builder.Services.AddGatewayServices();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Middleware order is IMPORTANT
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();