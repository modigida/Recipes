﻿using Microsoft.EntityFrameworkCore;
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
    public async Task<List<int>> GetTagsForRecipeAsync(int recipeId)
    {
        var allTags = await _context.RecipeTags.ToListAsync();

        var selectedTagIds = await _context.RecipeRecipeTags
            .Where(rrt => rrt.RecipeId == recipeId)
            .Select(rrt => rrt.RecipeTagId)
            .ToListAsync();

        return selectedTagIds;
    }
    public async Task SaveSelectedTagsAsync(int recipeId, List<int> selectedTagIds)
    {
        var existingLinks = await _context.RecipeRecipeTags
            .Where(rrt => rrt.RecipeId == recipeId)
            .ToListAsync();

        var linksToAdd = selectedTagIds
            .Where(tagId => !existingLinks.Any(link => link.RecipeTagId == tagId))
            .Select(tagId => new RecipeRecipeTags
            {
                RecipeId = recipeId,
                RecipeTagId = tagId
            });

        var linksToRemove = existingLinks
            .Where(link => !selectedTagIds.Contains(link.RecipeTagId));

        _context.RecipeRecipeTags.AddRange(linksToAdd);
        _context.RecipeRecipeTags.RemoveRange(linksToRemove);

        await _context.SaveChangesAsync();
    }
}

