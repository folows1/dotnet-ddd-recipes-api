using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.Recipe;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Domain.Services;
using MyRecipeBook.Infra.DataAccess;
using MyRecipeBook.Infra.DataAccess.Repos;
using MyRecipeBook.Infra.Extensions;
using MyRecipeBook.Infra.Security.Crypto;
using MyRecipeBook.Infra.Security.Tokens.Access.Generator;
using MyRecipeBook.Infra.Security.Tokens.Access.Validator;
using MyRecipeBook.Infra.Services;

namespace MyRecipeBook.Infra;

public static class DependencyInjectionExtension
{
    public static void AddInfra(this IServiceCollection services, IConfiguration cfg)
    {
        AddRepositories(services);
        AddTokens(services, cfg);
        AddLoggedUser(services);
        AddPwdEncripter(services, cfg);

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
        services.AddScoped<IRecipeWriteOnlyRepo, RecipeRepo>();
        services.AddScoped<IRecipeReadOnlyRepo, RecipeRepo>();
    }

    // private static void AddFluentMigrator_MySql(IServiceCollection services, IConfiguration cfg)
    // {
    //   var connectionString = cfg.ConnectionString();

    //   services.AddFluentMigratorCore().ConfigureRunner(options =>
    //   {
    //     options
    //       .AddMySql5()
    //       .WithGlobalConnectionString(connectionString)
    //       .ScanIn(Assembly.Load("MyRecipeBook.Infra")).For.All();
    //   });
    // }

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
}