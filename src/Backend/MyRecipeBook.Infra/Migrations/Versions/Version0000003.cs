using FluentMigrator;

namespace MyRecipeBook.Infra.Migrations.Versions;

[Migration(DatabaseVersions.SeedInitialData, "Seed initial users and recipes data")]
public class Version0000003 : Migration
{
    private const string UsersTable = "Users";
    private const string RecipesTable = "Recipes";
    private const string IngredientsTable = "Ingredients";
    private const string InstructionsTable = "Instructions";
    private const string DishTypesTable = "DishTypes";

    public override void Up()
    {
        SeedUsers();
        SeedRecipes();
    }

    public override void Down()
    {
    }

    private void SeedUsers()
    {
        const string passwordHashReal =
            "fb4d6f9bf0ef2e634b78cafa40e6e8921f95facd13fc3435ee42c3522f2526458b8928475ded872684c6e547ba96afaf4aa6fe5bab34211615adf9ad749ec60f";

        for (var i = 1; i <= 10; i++)
        {
            var email = $"usuario{i}@seed.com";

            Execute.Sql($@"
IF NOT EXISTS (SELECT 1 FROM {UsersTable} WHERE Email = '{email}')
BEGIN
    INSERT INTO {UsersTable} (Name, Email, Password, UserIdentifier, Active, CreatedOn)
    VALUES ('Usuário Seed {i}', '{email}', '{passwordHashReal}', NEWID(), 1, GETUTCDATE());
END");
        }
    }

    private void SeedRecipes()
    {
        for (var i = 1; i <= 10; i++)
        {
            var email = $"usuario{i}@seed.com";

            Execute.Sql($@"
DECLARE @UserId BIGINT;
SELECT @UserId = Id FROM {UsersTable} WHERE Email = '{email}';

IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM {RecipesTable} WHERE Title = 'Receita Seed {i}' AND UserId = @UserId)
BEGIN
    INSERT INTO {RecipesTable} (Title, CookingTime, Difficulty, UserId, Active, CreatedOn)
    VALUES ('Receita Seed {i}', {i % 3}, {i % 3}, @UserId, 1, GETUTCDATE());

    DECLARE @RecipeId BIGINT = SCOPE_IDENTITY();

    INSERT INTO {IngredientsTable} (Item, RecipeId, Active, CreatedOn)
    VALUES ('Ingrediente 1 da receita {i}', @RecipeId, 1, GETUTCDATE()),
           ('Ingrediente 2 da receita {i}', @RecipeId, 1, GETUTCDATE());

    INSERT INTO {InstructionsTable} (Step, Text, RecipeId, Active, CreatedOn)
    VALUES (1, 'Passo 1 da receita {i}', @RecipeId, 1, GETUTCDATE()),
           (2, 'Passo 2 da receita {i}', @RecipeId, 1, GETUTCDATE());

    INSERT INTO {DishTypesTable} (Type, RecipeId, Active, CreatedOn)
    VALUES ({i % 3}, @RecipeId, 1, GETUTCDATE());
END");
        }
    }
}