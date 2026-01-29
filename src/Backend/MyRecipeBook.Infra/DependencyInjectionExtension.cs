using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Repos;
using MyRecipeBook.Domain.Repos.User;
using MyRecipeBook.Infra.DataAccess;
using MyRecipeBook.Infra.DataAccess.Repos;

namespace MyRecipeBook.Infra;

public static class DependencyInjectionExtension
{
  public static void AddInfra(this IServiceCollection services, IConfiguration cfg)
  {

    var dbType = cfg.GetConnectionString("DatabaseType");

    var dbTypeEnum = (DatabaseType)Enum.Parse(typeof(DatabaseType), dbType!);

    if (dbTypeEnum == DatabaseType.MySql)
    {
      //
    }
    else
    {
      AddDbContext_SqlServer(services, cfg);
    }

    AddRepositories(services);
  }

  private static void AddDbContext_SqlServer(IServiceCollection services, IConfiguration cfg)
  {
    var connectionString = cfg.GetConnectionString("ConnectionSQLServer");

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
}
