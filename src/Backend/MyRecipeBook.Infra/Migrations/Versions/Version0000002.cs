using FluentMigrator;

namespace MyRecipeBook.Infra.Migrations.Versions;

[Migration(DatabaseVersions.TableRecipes, "Create table to save the recipes information")]
public class Version0000002 : VersionBase
{
    private const string RecipeTableName = "Recipes";

    public override void Up()
    {
        CreateTable(RecipeTableName)
            .WithColumn("Title").AsString().NotNullable()
            .WithColumn("CookingTime").AsInt32().Nullable()
            .WithColumn("Difficulty").AsInt32().Nullable()
            .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("FK_Recipe_User_Id", "Users", "Id");

        CreateTable("Ingredients")
            .WithColumn("Item").AsString().NotNullable()
            .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_Ingredient_Recipe_Id", RecipeTableName, "Id")
            .OnDelete(System.Data.Rule.Cascade);

        CreateTable("Instructions")
            .WithColumn("Step").AsInt32().NotNullable()
            .WithColumn("Text").AsString(2000).NotNullable()
            .WithColumn("RecipeId").AsInt64().NotNullable()
            .ForeignKey("FK_Instruction_Recipe_Id", RecipeTableName, "Id")
            .OnDelete(System.Data.Rule.Cascade);

        CreateTable("DishTypes")
            .WithColumn("Type").AsInt32().NotNullable()
            .WithColumn("RecipeId").AsInt64().NotNullable().ForeignKey("FK_DishType_Recipe_Id", RecipeTableName, "Id")
            .OnDelete(System.Data.Rule.Cascade);
    }
}