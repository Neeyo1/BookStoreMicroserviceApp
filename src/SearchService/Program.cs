using MassTransit;
using Polly;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<ISearchRepository, SearchRepository>();

builder.Services.AddMassTransit(x => 
{
    x.AddConsumersFromNamespaceContaining<BookCreatedConsumer>();
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, conf) =>
    {
        conf.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest")!);
            host.Password(builder.Configuration.GetValue("RabbitMq:Username", "guest")!);
        });
        
        conf.ReceiveEndpoint("search-book-created", y =>
        {
            y.UseMessageRetry(z => z.Interval(5, 5));
            y.ConfigureConsumer<BookCreatedConsumer>(context);
        });

        conf.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

var retryPolicy = Policy
    .Handle<TimeoutException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10));

retryPolicy.ExecuteAndCapture(async () => 
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var connectionString = app.Configuration.GetConnectionString("MongoDbConnection");
    var logger = services.GetRequiredService<ILogger<DbInitializer>>();
    await DbInitializer.InitDb(connectionString!, logger);
});

app.Run();

public partial class Program {}
