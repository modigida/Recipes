using Microsoft.EntityFrameworkCore;
using Recipes.Database;

namespace Recipes.Services;
public class RecipeService
{
    private readonly AppDbContext _context;
    public RecipeService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Model.Recipes>> GetAllRecipesAsync()
    {
        return await _context.Recipes
        .Include(r => r.RecipeRecipeTags)
            .ThenInclude(r => r.RecipeTag)
        .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
        .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Unit)
        .Include(r => r.CookingTime)
        .ToListAsync();
    }
    public async Task<Model.Recipes?> GetRecipeByIdAsync(int id)
    {
        return await _context.Recipes
        .Include(r => r.RecipeRecipeTags)
            .ThenInclude(r => r.RecipeTag)
        .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
        .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Unit)
        .Include(r => r.CookingTime)
        .FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<int> AddRecipeAsync(Model.Recipes recipe)
    {
        var newRecipe = new Model.Recipes
        {
            Recipe = recipe.Recipe,
            CookingInstructions = recipe.CookingInstructions,
            CookingTimeId = recipe.CookingTimeId,
            IsFavorite = recipe.IsFavorite
        };

        _context.Recipes.Add(newRecipe);
        await _context.SaveChangesAsync();
        return newRecipe.Id;
    }
    public async Task UpdateRecipeAsync(Model.Recipes recipe)
    {
        var existingRecipe = await _context.Recipes.FindAsync(recipe.Id);
        if (existingRecipe != null)
        {
            existingRecipe.Recipe = recipe.Recipe;
            existingRecipe.CookingInstructions = recipe.CookingInstructions;
            existingRecipe.CookingTimeId = recipe.CookingTimeId;
            existingRecipe.IsFavorite = recipe.IsFavorite;

            await _context.SaveChangesAsync();
        }
    }
    public async Task DeleteRecipeAsync(int id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe != null)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }
    }
}
