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

    public ObservableCollection<Model.Recipes> Recipes { get; set; }
    
    public IEnumerable<string> SortOptions { get; } = new List<string> { "Sort by..", "Name", "Favorite", "Cooking Time" };

    private string _selectedSortOption = "Sort by..";
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            _selectedSortOption = value;
            OnPropertyChanged();
            SortRecipes();
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
            recipe.RecipeTags = string.Join(", ",
            recipe.RecipeRecipeTags.Select(rrt => rrt.RecipeTag.Tag));
            recipe.CookingTime = DetailedRecipeViewModel.CookingTimes.FirstOrDefault(ct => ct.Id == recipe.CookingTimeId);

            Recipes.Add(recipe);

        }
        OnPropertyChanged(nameof(Recipes));
    }
    private void SortRecipes()
    {
        if (string.IsNullOrEmpty(SelectedSortOption))
            return;

        IEnumerable<Model.Recipes> sortedRecipes = SelectedSortOption switch
        {
            "Name" => Recipes.OrderBy(r => r.Recipe),
            "Favorite" => Recipes.OrderByDescending(r => r.IsFavorite),
            "Cooking Time" => Recipes.OrderBy(r => r.CookingTimeId),
            _ => Recipes
        };

        Recipes = new ObservableCollection<Model.Recipes>(sortedRecipes);
        OnPropertyChanged(nameof(Recipes));
    }
}
