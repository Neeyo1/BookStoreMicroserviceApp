using AutoMapper;
using Elasticsearch.Net;
using MassTransit;
using Nest;
using Polly;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Entities;
using SearchService.Interfaces;
using StackExchange.Redis;

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
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest")!);
        });
        
        conf.ReceiveEndpoint("search-book-created", y =>
        {
            y.UseMessageRetry(z => z.Interval(5, 5));
            y.ConfigureConsumer<BookCreatedConsumer>(context);
        });

        conf.ConfigureEndpoints(context);
    });
});

var retryPolicy = Polly.Policy
    .Handle<TimeoutException>()
    .Or<RedisConnectionException>()
    .Or<RedisTimeoutException>()
    .Or<RedisServerException>()
    .Or<ElasticsearchClientException>()
    .Or<UnexpectedElasticsearchClientException>()
    .Or<Elasticsearch.Net.PipelineException>()
    .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(10));

builder.Services.AddStackExchangeRedisCache(options =>
{
    retryPolicy.Execute(() => 
    {
        var host = builder.Configuration.GetValue("Redis:Host", "localhost");
        var password = builder.Configuration.GetValue("Redis:Password", "secretpassword");

        options.Configuration = $"{host}:6379,password={password}";
        options.InstanceName = "BookStore_";
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    retryPolicy.Execute(() => 
    {
        var host = builder.Configuration.GetValue("Redis:Host", "localhost");
        var password = builder.Configuration.GetValue("Redis:Password", "secretpassword");
        return ConnectionMultiplexer.Connect($"{host}:6379,password={password}");
    })
);

builder.Services.AddSingleton<IElasticClient>(_ =>
    retryPolicy.Execute(() => 
    {
        var uri = builder.Configuration.GetValue("Elasticsearch:Uri", "http://localhost:9200")!;
        var settings = new ConnectionSettings(new Uri(uri))
            .DefaultIndex("books")
            .DisableDirectStreaming()
            .PrettyJson();

        return new ElasticClient(settings);
    })
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

retryPolicy.ExecuteAndCapture(async () => 
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var connectionString = app.Configuration.GetConnectionString("MongoDbConnection");
    var logger = services.GetRequiredService<ILogger<DbInitializer>>();
    await DbInitializer.InitDb(connectionString!, logger);
});

retryPolicy.ExecuteAndCapture(async () => 
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var client = services.GetRequiredService<IElasticClient>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    var indexExists = client.Indices.Exists("books").Exists;
    if (!indexExists)
    {
        var createIndexResponse = client.Indices.Create("books", x => x
            .Map<BookES>(book => book
                .AutoMap()
            )
        );

        if (createIndexResponse.IsValid)
        {
            indexExists = true;
            logger.LogInformation("------ Successfully created index books in Elasticsearch ------");
        }
        else
        {
            logger.LogError("------ Failed to create index books in Elasticsearch ------");
        }
    }
    else
    {
        logger.LogInformation("------ Successfully created index books in Elasticsearch ------");
    }

    if(indexExists)
    {
        var elasticClient = services.GetRequiredService<IElasticClient>();
        var mapper = services.GetRequiredService<IMapper>();
        var indexLogger = services.GetRequiredService<ILogger<DbInitializer>>();
        await DbInitializer.IndexDocuments(elasticClient, mapper, indexLogger);
    }
});

app.Run();

public partial class Program {}
