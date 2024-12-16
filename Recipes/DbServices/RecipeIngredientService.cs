using Microsoft.EntityFrameworkCore;
using Recipes.Database;
using Recipes.Model;

namespace Recipes.Services;
public class RecipeIngredientService
{
    private readonly AppDbContext _context;
    public RecipeIngredientService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<RecipeIngredients>> GetIngredientsByRecipeIdAsync(int recipeId)
    {
        try
        {
            return await _context.RecipeIngredients
                .Include(ri => ri.Ingredient)
                .Include(ri => ri.Unit)
                .Where(ri => ri.RecipeId == recipeId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching ingredients for recipe ID ({recipeId}): {ex.Message}");
            return new List<RecipeIngredients>();
        }
    }
    public async Task<bool> IsIngredientUsedAsync(int ingredientId)
    {
        try
        {
            return await _context.RecipeIngredients.AnyAsync(ri => ri.IngredientId == ingredientId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking if ingredient is used (ID: {ingredientId}): {ex.Message}");
            return false;
        }
    }
    public async Task AddRecipeIngredientAsync(int recipeId, int ingredientId, double quantity, int unitId)
    {
        try
        {
            var recipeIngredient = new RecipeIngredients
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Quantity = quantity,
                UnitId = unitId
            };

            _context.RecipeIngredients.Add(recipeIngredient);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding recipe ingredient (RecipeID: {recipeId}, IngredientID: {ingredientId}): {ex.Message}");
        }
    }

    public async Task UpdateRecipeIngredientAsync(int recipeId, int ingredientId, double quantity, int unitId)
    {
        try
        {
            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId);

            if (recipeIngredient != null)
            {
                recipeIngredient.Quantity = quantity;
                recipeIngredient.UnitId = unitId;

                _context.RecipeIngredients.Update(recipeIngredient);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine($"RecipeIngredient not found (RecipeID: {recipeId}, IngredientID: {ingredientId}).");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating recipe ingredient (RecipeID: {recipeId}, IngredientID: {ingredientId}): {ex.Message}");
        }
    }

    public async Task RemoveRecipeIngredientAsync(int recipeId, int ingredientId)
    {
        try
        {
            var recipeIngredient = await _context.RecipeIngredients
                .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId);

            if (recipeIngredient != null)
            {
                _context.RecipeIngredients.Remove(recipeIngredient);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing recipe ingredient (RecipeID: {recipeId}, IngredientID: {ingredientId}): {ex.Message}");
        }
    }
}
