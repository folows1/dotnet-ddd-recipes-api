using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repos.Recipe;

namespace MyRecipeBook.Infra.DataAccess.Repos;

public class RecipeRepo(MyRecipeBookDbContext dbContext)
    : IRecipeWriteOnlyRepo, IRecipeReadOnlyRepo, IRecipeUpdateOnlyRepo
{
    public async Task Add(Recipe recipe)
    {
        await dbContext.Recipes.AddAsync(recipe);
    }

    public async Task Delete(long recipeId)
    {
        var recipe = await dbContext.Recipes.FindAsync(recipeId);

        dbContext.Recipes.Remove(recipe!);
    }

    public async Task<IList<Recipe>> Filter(User user, FilterRecipesDto filters)
    {
        var query = dbContext
            .Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Where(recipe => recipe.Active && recipe.UserId == user.Id);

        if (filters.Difficulties.Any())
        {
            query = query.Where(recipe => recipe.Difficulty.HasValue &&
                                          filters.Difficulties.Contains(recipe.Difficulty.Value));
        }

        if (filters.CookingTimes.Any())
        {
            query = query.Where(recipe => recipe.CookingTime.HasValue &&
                                          filters.CookingTimes.Contains(recipe.CookingTime.Value));
        }

        if (filters.DishTypes.Any())
        {
            query = query.Where(recipe => recipe.DishTypes.Any(dishType =>
                filters.DishTypes.Contains(dishType.Type)));
        }

        if (filters.RecipeTitleOrIngredient.NotEmpty())
        {
            query = query.Where(recipe => recipe.Title.Contains(filters.RecipeTitleOrIngredient)
                                          || recipe.Ingredients.Any(ingredient =>
                                              ingredient.Item.Contains(filters.RecipeTitleOrIngredient))
            );
        }

        return await query.ToListAsync();
    }

    async Task<Recipe?> IRecipeReadOnlyRepo.GetById(User user, long recipeId)
    {
        return await GetFullRecipe().AsNoTracking()
            .FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == user.Id);
    }

    async Task<Recipe?> IRecipeUpdateOnlyRepo.GetById(User user, long recipeId)
    {
        return await GetFullRecipe()
            .FirstOrDefaultAsync(recipe => recipe.Active && recipe.Id == recipeId && recipe.UserId == user.Id);
    }

    private IIncludableQueryable<Recipe, IList<Instruction>> GetFullRecipe()
    {
        return dbContext
            .Recipes
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.DishTypes)
            .Include(recipe => recipe.Instructions);
    }

    public void Update(Recipe recipe) => dbContext.Recipes.Update(recipe);
}