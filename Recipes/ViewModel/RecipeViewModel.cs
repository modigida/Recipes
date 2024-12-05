using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Recipes.Commands;
using Recipes.Services;
using Recipes.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Recipes.ViewModel;
public class RecipeViewModel : BaseViewModel
{
    private readonly RecipeService _recipeService;

    
    public ObservableCollection<Model.Recipes> Recipes { get; set; } = new();
    
    public RecipeViewModel(RecipeService recipeService)
    {
        _recipeService = recipeService;

        LoadRecipes();
    }

    private async void LoadRecipes()
    {
        Recipes.Clear();

        var recipes = await _recipeService.GetAllRecipesAsync(); 
        foreach (var recipe in recipes)
        {
            Recipes.Add(recipe);
        }
    }
    
}
