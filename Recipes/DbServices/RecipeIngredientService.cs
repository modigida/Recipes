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
        return await _context.RecipeIngredients
            .Include(ri => ri.Ingredient)
            .Include(ri => ri.Unit)
            .Where(ri => ri.RecipeId == recipeId)
            .ToListAsync();
    }
    public async Task<bool> IsIngredientUsedAsync(int ingredientId)
    {
        return await _context.RecipeIngredients.AnyAsync(ri => ri.IngredientId == ingredientId);
    }
    public async Task AddRecipeIngredientAsync(int recipeId, int ingredientId, string quantity, int unitId)
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

    public async Task UpdateRecipeIngredientAsync(int recipeId, int ingredientId, string quantity, int unitId)
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
            throw new Exception("RecipeIngredient not found.");
        }
    }

    public async Task RemoveRecipeIngredientAsync(int recipeId, int ingredientId)
    {
        var recipeIngredient = await _context.RecipeIngredients
            .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId);

        if (recipeIngredient != null)
        {
            _context.RecipeIngredients.Remove(recipeIngredient);
            await _context.SaveChangesAsync();
        }
    }
}
