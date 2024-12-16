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

    // Only get data needed for the RecipeView
    public async Task<List<Model.Recipes>> GetAllRecipesAsync()
    {
        try
        {
             return await _context.Recipes
                .Include(r => r.RecipeRecipeTags)
                    .ThenInclude(r => r.RecipeTag)
                .Include(r => r.CookingTime)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching all recipes: {ex.Message}");
            return new List<Model.Recipes>();
        }
    }

    // Get all data for a single recipe
    public async Task<Model.Recipes?> GetRecipeByIdAsync(int id)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching recipe with ID {id}: {ex.Message}");
            return new Model.Recipes();
        }
    }
    public async Task<int> AddRecipeAsync(Model.Recipes recipe)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding a new recipe: {ex.Message}");
            throw;
        }
    }
    public async Task UpdateRecipeAsync(Model.Recipes recipe)
    {
        try
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
            else
            {
                Console.WriteLine($"Recipe with ID {recipe.Id} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating recipe with ID {recipe.Id}: {ex.Message}");
        }
    }
    public async Task DeleteRecipeAsync(int id)
    {
        try
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"Recipe with ID {id} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting recipe with ID {id}: {ex.Message}");
        }
    }
}
