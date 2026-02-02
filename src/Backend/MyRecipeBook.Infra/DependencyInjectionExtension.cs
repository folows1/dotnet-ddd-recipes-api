using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Infra.DataAccess;
using MyRecipeBook.Infra.DataAccess.Repos;
using MyRecipeBook.Infra.Extensions;

namespace MyRecipeBook.Infra;

public static class DependencyInjectionExtension
{
  public static void AddInfra(this IServiceCollection services, IConfiguration cfg)
  {
    AddRepositories(services);
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
    services.AddScoped<IUserWriteOnlyRepo, UserRepo>();
    services.AddScoped<IUserReadOnlyRepo, UserRepo>();
    services.AddScoped<IUnitOfWork, UnitOfWork>();
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
}
