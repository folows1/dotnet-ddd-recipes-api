using System.Reflection;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Domain.Services.OpenAI;
using MyRecipeBook.Domain.Services.ServiceBus;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;
using MyRecipeBook.Infra.DataAccess;
using MyRecipeBook.Infra.DataAccess.Repos;
using MyRecipeBook.Infra.Extensions;
using MyRecipeBook.Infra.Security.Crypto;
using MyRecipeBook.Infra.Security.Tokens.Access.Generator;
using MyRecipeBook.Infra.Security.Tokens.Access.Validator;
using MyRecipeBook.Infra.Services;
using MyRecipeBook.Infra.Services.OpenAI;
using MyRecipeBook.Infra.Services.ServiceBus;
using MyRecipeBook.Infra.Services.Storage;
using OpenAI.Chat;

namespace MyRecipeBook.Infra;

public static class DependencyInjectionExtension
{
    public static void AddInfra(this IServiceCollection services, IConfiguration cfg)
    {
        AddPwdEncripter(services, cfg);
        AddRepositories(services);
        AddLoggedUser(services);
        AddTokens(services, cfg);
        AddOpenAi(services, cfg);
        AddAzureStorage(services, cfg);
        AddQueueAndProcessor(services, cfg);

        if (cfg.IsUnitTestEnv())
            return;

        var dbType = cfg.DatabaseType();

        if (dbType == DatabaseType.MySql)
        {
            //
        }
        else
        {
            AddDbContext_SqlServer(services, cfg);
            AddFluentMigrator_SqlServer(services, cfg);
        }
    }

    private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration cfg)
    {
        var connectionString = cfg.ConnectionString();

        services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions =>
        {
            dbContextOptions.UseSqlServer(connectionString);
        });
    }

    // private static void AddDbContextMySqlServer(IServiceCollection services)
    // {
    //   var connectionString = "";
    //   var serverV = new MySqlServerVersion(new Version(8, 0, 35)) -> MysqlWorkbench version -> 8.0.35

    //   services.AddDbContext<MyRecipeBookDbContext>(dbContextOptions =>
    //   {
    //     dbContextOptions.UseMySql(connectionString, serverV); Pomelo.EntityFrameworkCore.MySql 8.0.0
    //   });
    // }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IUserWriteOnlyRepo, UserRepo>();
        services.AddScoped<IUserReadOnlyRepo, UserRepo>();
        services.AddScoped<IUserUpdateOnlyRepo, UserRepo>();
        services.AddScoped<IUserDeleteOnlyRepo, UserRepo>();
        services.AddScoped<IRecipeWriteOnlyRepo, RecipeRepo>();
        services.AddScoped<IRecipeReadOnlyRepo, RecipeRepo>();
        services.AddScoped<IRecipeUpdateOnlyRepo, RecipeRepo>();
    }

    private static void AddFluentMigrator_SqlServer(IServiceCollection services, IConfiguration cfg)
    {
        var connectionString = cfg.ConnectionString();

        services.AddFluentMigratorCore().ConfigureRunner(options =>
        {
            options
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("MyRecipeBook.Infra")).For.All();
        });
    }

    private static void AddTokens(IServiceCollection services, IConfiguration cfg)
    {
        var expirationTimeMinutes = cfg.GetValue<uint>("Settings:Jwt:ExpirationTimeMinutes");
        var signingKey = cfg.GetValue<string>("Settings:Jwt:SigningKey");

        services.AddScoped<IAccessTokenGenerator>(_ => new JwtTokenGenerator(expirationTimeMinutes, signingKey!));
        services.AddScoped<IAccessTokenValidator>(_ => new JwtTokenValidator(signingKey!));
    }

    private static void AddLoggedUser(IServiceCollection services)
    {
        services.AddScoped<ILoggedUser, LoggedUser>();
    }

    private static void AddPwdEncripter(IServiceCollection services, IConfiguration cfg)
    {
        var key = cfg.GetValue<string>("Settings:Password:AdditionalKey");

        services.AddScoped<IPasswordEncripter>(_ => new Sha512Encripter(key!));
    }

    private static void AddOpenAi(IServiceCollection svc, IConfiguration cfg)
    {
        svc.AddScoped<IGenerateRecipeAI, ChatGptService>();

        var key = cfg.GetValue<string>("Settings:OpenAI:ApiKey");

        svc.AddScoped(_ => new ChatClient(MyRecipeBookRuleConstants.ChatModel, key!));
    }

    private static void AddAzureStorage(IServiceCollection services, IConfiguration cfg)
    {
        var connectionString = cfg.GetValue<string>("Settings:BlobStorage:Azure");

        if (connectionString.NotEmpty())
            services.AddScoped<IBlobStorageService>(_ =>
                new AzureStorageService(new BlobServiceClient(connectionString)));
    }

    private static void AddQueueAndProcessor(IServiceCollection services, IConfiguration cfg)
    {
        var connectionString = cfg.GetValue<string>("Settings:ServiceBus:DeleteUser");

        if (connectionString.NotEmpty().IsFalse()) return;

        var client = new ServiceBusClient(connectionString!, new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        });

        var deleteQueue = new DeleteUserQueue(client.CreateSender("user"));

        var deleteUserProcessor = new DeleteUserProcessor(client.CreateProcessor("user", new
            ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
            }));

        services.AddSingleton(deleteUserProcessor);
        services.AddScoped<IDeleteUserQueue>(_ => deleteQueue);
    }
}