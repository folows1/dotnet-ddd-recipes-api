using FluentMigrator;

namespace MyRecipeBook.Infra.Migrations.Versions;

[Migration(DatabaseVersions.ImageForRecipes, "Add column on recipe table to save images")]
public class Version0000004 : VersionBase
{
    private const string RecipeTableName = "Recipes";

    public override void Up()
    {
        Alter.Table("Recipes").AddColumn("ImageIdentifier").AsString().Nullable();
    }
}