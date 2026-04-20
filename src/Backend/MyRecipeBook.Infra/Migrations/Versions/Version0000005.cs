using FluentMigrator;

namespace MyRecipeBook.Infra.Migrations.Versions;

[Migration(DatabaseVersions.TableRefreshToken, "Create table to save refresh token")]
public class Version0000005 : VersionBase
{
    public override void Up()
    {
        CreateTable("RefreshToken")
            .WithColumn("Value").AsString().NotNullable()
            .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_RefreshTokens_User_Id", "Users", "Id");
    }
}