using CartService.Consumers;
using CartService.Data;
using CartService.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddDbContext<CartDbContext>(opt => 
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMassTransit(x => 
{
    x.AddConsumersFromNamespaceContaining<BookCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("carts", false));

    x.AddEntityFrameworkOutbox<CartDbContext>(y =>
    {
        y.QueryDelay = TimeSpan.FromSeconds(10);
        y.UsePostgres();
        y.UseBusOutbox();
    });

    x.UsingRabbitMq((context, conf) =>
    {
        conf.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest")!);
            host.Password(builder.Configuration.GetValue("RabbitMq:Username", "guest")!);
        });
        
        conf.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["IdentityServiceUrl"];
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters.ValidateAudience = false;
            options.TokenValidationParameters.NameClaimType = "username";
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var retryPolicy = Policy
    .Handle<NpgsqlException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(5));

retryPolicy.ExecuteAndCapture(async () => 
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CartDbContext>();
    var logger = services.GetRequiredService<ILogger<DbInitializer>>();
    await context.Database.MigrateAsync();
    await DbInitializer.InitDb(context, logger);
});

app.Run();

public partial class Program {}
