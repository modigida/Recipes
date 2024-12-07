using Recipes.Services;
using System.Collections.ObjectModel;

namespace Recipes.ViewModel;
public class RecipeViewModel : BaseViewModel
{
    private readonly RecipeService _recipeService;
    private readonly MainWindowViewModel _mainWindowViewModel;

    private Model.Recipes _selectedRecipe;
    public Model.Recipes SelectedRecipe
    {
        get => _selectedRecipe;
        set
        {
            _selectedRecipe = value;
            OnPropertyChanged();

            if (_selectedRecipe != null)
            {
                _mainWindowViewModel.OpenDetailedRecipe();
            }
        }
    }

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
    public RecipeViewModel(RecipeService recipeService, MainWindowViewModel mainWindowViewModel)
    {
        _recipeService = recipeService;
        _mainWindowViewModel = mainWindowViewModel;

        Recipes = new ObservableCollection<Model.Recipes>();

        LoadRecipes();
    }
    public async void LoadRecipes()
    {
        Recipes.Clear();

        var recipes = await _recipeService.GetAllRecipesAsync(); 
        foreach (var recipe in recipes)
        {
            Recipes.Add(recipe);
            OnPropertyChanged(nameof(Recipes));
        }
    }
}
