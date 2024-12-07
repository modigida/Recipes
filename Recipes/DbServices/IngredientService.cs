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
        return await _context.Ingredients.ToListAsync();
    }
    public async Task<Ingredients?> GetIngredientByIdAsync(int id)
    {
        return await _context.Ingredients.FindAsync(id);
    }
    public async Task<int> AddIngredientAsync(Ingredients ingredient)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ingredient.Id;
    }
    public async Task<bool> IngredientExistsAsync(string ingredientName)
    {
        return await _context.Ingredients
            .AnyAsync(i => i.Ingredient.ToLower() == ingredientName.ToLower());
    }
    public async Task UpdateIngredientAsync(Ingredients ingredient)
    {
        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteIngredientAsync(int id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient != null)
        {
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
        }
    }
}
