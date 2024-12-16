using Microsoft.EntityFrameworkCore;
using Recipes.Database;
using Recipes.Model;

namespace Recipes.Services;
public class IngredientService
{
    private readonly AppDbContext _context;
    public IngredientService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Ingredients>> GetAllIngredientsAsync()
    {
        try
        {
            return await _context.Ingredients.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching all ingredients: {ex.Message}");
            return new List<Ingredients>();
        }
    }
    public async Task<Ingredients?> GetIngredientByIdAsync(int id)
    {
        try
        {
            return await _context.Ingredients.FindAsync(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching ingredient by ID ({id}): {ex.Message}");
            return null;
        }
    }
    public async Task<Ingredients?> GetIngredientByNameAsync(string ingredientName)
    {
        try
        {
            return await _context.Ingredients.FirstOrDefaultAsync(i => i.Ingredient == ingredientName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching ingredient by name ({ingredientName}): {ex.Message}");
            return null;
        }
    }
    public void AttachIngredient(Ingredients ingredient)
    {
        try
        {
            _context.Ingredients.Attach(ingredient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error attaching ingredient: {ex.Message}");
        }
    }

    public async Task<int> AddIngredientAsync(Ingredients ingredient)
    {
        try
        {
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding ingredient: {ex.Message}");
            return -1;
        }
    }
    public async Task<bool> IngredientExistsAsync(string ingredientName)
    {
        try
        {
            return await _context.Ingredients
                .AnyAsync(i => i.Ingredient.ToLower() == ingredientName.ToLower());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking if ingredient exists ({ingredientName}): {ex.Message}");
            return false;
        }
    }
    public async Task UpdateIngredientAsync(Ingredients ingredient)
    {
        try
        {
            _context.Ingredients.Update(ingredient);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating ingredient: {ex.Message}");
        }
    }
    public async Task DeleteIngredientAsync(int id)
    {
        try
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting ingredient with ID ({id}): {ex.Message}");
        }
    }
}
