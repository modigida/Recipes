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
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddRecipeAsync(Model.Recipes recipe)
    {
        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRecipeAsync(Model.Recipes recipe)
    {
        _context.Recipes.Update(recipe);
        await _context.SaveChangesAsync();
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
