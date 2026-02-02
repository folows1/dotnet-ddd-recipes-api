using Microsoft.Extensions.Configuration;
using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Infra.Extensions;

public static class ConfigurationExtension
{

  public static bool IsUnitTestEnv(this IConfiguration configuration)
  {
    return configuration.GetValue<bool>("InMemoryTest");
  }
  
  public static string ConnectionString(this IConfiguration cfg)
  {
    var databaseType = cfg.DatabaseType();

    if (databaseType == Domain.Enums.DatabaseType.MySql)
      return cfg.GetConnectionString("ConnectionMySqlServer")!;
    else
      return cfg.GetConnectionString("ConnectionSQLServer")!;
  }

  public static DatabaseType DatabaseType(this IConfiguration cfg)
  {
    var dbType = cfg.GetConnectionString("DatabaseType");

    return (DatabaseType)Enum.Parse(typeof(DatabaseType), dbType!);
  }
}
