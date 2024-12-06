using Recipes.Services;
using System.Collections.ObjectModel;

namespace Recipes.ViewModel;
public class RecipeViewModel : BaseViewModel
{
    private readonly RecipeService _recipeService;


    private ObservableCollection<Model.Recipes> _recipes;
    public ObservableCollection<Model.Recipes> Recipes
    {
        get { return _recipes; }
        set
        {
            _recipes = value;
            OnPropertyChanged();
        }
    }
    
    public RecipeViewModel(RecipeService recipeService)
    {
        _recipeService = recipeService;

        Recipes = new ObservableCollection<Model.Recipes>();

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
