using Microsoft.EntityFrameworkCore;
using Recipes.Database;
using Recipes.Model;

namespace Recipes.Services;
public class TagService
{
    private readonly AppDbContext _context;

    public TagService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecipeTags>> GetTagsForRecipeAsync(int recipeId)
    {
        // Hämta alla taggar
        var allTags = await _context.RecipeTags.ToListAsync();

        // Hämta valda taggars ID för receptet
        var selectedTagIds = await _context.RecipeRecipeTags
            .Where(rrt => rrt.RecipeId == recipeId)
            .Select(rrt => rrt.RecipeTagId)
            .ToListAsync();

        // Markera taggar som valda genom att jämföra ID
        foreach (var tag in allTags)
        {
            tag.Tag += selectedTagIds.Contains(tag.Id) ? " (Selected)" : ""; // Exempel för visualisering
        }

        return allTags;
    }

    public async Task SaveSelectedTagsAsync(int recipeId, List<int> selectedTagIds)
    {
        // Hämta befintliga kopplingar
        var existingLinks = await _context.RecipeRecipeTags
            .Where(rrt => rrt.RecipeId == recipeId)
            .ToListAsync();

        // Lägg till nya kopplingar
        var linksToAdd = selectedTagIds
            .Where(tagId => !existingLinks.Any(link => link.RecipeTagId == tagId))
            .Select(tagId => new RecipeRecipeTags
            {
                RecipeId = recipeId,
                RecipeTagId = tagId
            });

        // Ta bort kopplingar som inte längre är valda
        var linksToRemove = existingLinks
            .Where(link => !selectedTagIds.Contains(link.RecipeTagId));

        _context.RecipeRecipeTags.AddRange(linksToAdd);
        _context.RecipeRecipeTags.RemoveRange(linksToRemove);

        await _context.SaveChangesAsync();
    }
}

