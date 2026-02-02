using Dapper;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Enums;
using MyRecipeBook.Domain.Extensions;

namespace MyRecipeBook.Infra.Migrations;

public static class DatabaseMigration
{
  public static void Migrate(DatabaseType dbType, string connectionString, IServiceProvider provider)
  {
    if (dbType == DatabaseType.MySql)
      EnsureDatabaseCreated_MySql(connectionString);
    else
      EnsureDatabaseCreated_SQLServer(connectionString);

    MigrationDatabase(provider);
  }

  private static void EnsureDatabaseCreated_SQLServer(string connectionString)
  {
    var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

    var databaseName = connectionStringBuilder.InitialCatalog;

    connectionStringBuilder.Remove("Database");

    using var dbConnection = new SqlConnection(connectionStringBuilder.ConnectionString);

    var parameters = new DynamicParameters();
    parameters.Add("name", databaseName);
    var records = dbConnection.Query("SELECT * FROM sys.databases WHERE name = @name", parameters);

    if (records.Any().IsFalse())
      dbConnection.Execute($"CREATE DATABASE {databaseName}");
  }

  private static void EnsureDatabaseCreated_MySql(string connectionString)
  {

    // var connectionStringBuilder = new MySQLConnectionString(connectionString);
    // var databaseName = connectionStringBuilder.Database;

    // connectionStringBuilder.Remove("Database");

    // using var dbConnection = new MySqlConnection(connectionStringBuilder.ConnectionString);

    // var parameters = new DynamicParameters();
    // parameters.Add("name", databaseName);
    // var records = dbConnection.Query("SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = @name", parameters);

    // if (records.Any() == false)
    //   dbConnection.Execute($"CREATE DATABASE {databaseName}");
  }

  private static void MigrationDatabase(IServiceProvider provider)
  {
    var runner = provider.GetRequiredService<IMigrationRunner>();
    runner.ListMigrations();

    runner.MigrateUp();
  }
}
