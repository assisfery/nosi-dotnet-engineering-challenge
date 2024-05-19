using NOS.Engineering.Challenge.API.Extensions;

var builder = WebApplication.CreateBuilder(args)
        .ConfigureWebHost()
        .RegisterServices();

// Add log service
builder.Logging.AddConsole();

var app = builder.Build();

app.MapControllers();
app.UseSwagger()
    .UseSwaggerUI();
    
app.Run();