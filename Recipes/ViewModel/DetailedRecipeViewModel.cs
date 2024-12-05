using Recipes.Commands;
using Recipes.Model;
using Recipes.Services;
using Recipes.View;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Recipes.ViewModel;
public class DetailedRecipeViewModel : BaseViewModel
{
    private readonly GetStaticListDataService _staticDataService;
    private readonly IngredientService _ingredientService;

    public ObservableCollection<Ingredients> AllIngredients { get; set; }
    public ObservableCollection<Ingredients> FilteredIngredients { get; set; }
    public ObservableCollection<Units> Units { get; set; }
    public ObservableCollection<CookingTimes> CookingTimes { get; set; }
    public ObservableCollection<RecipeTags> RecipeTags { get; set; }

    private Model.Recipes _recipe;
    public Model.Recipes Recipe
    {
        get => _recipe;
        set 
        {
            _recipe = value; 
            OnPropertyChanged();
        } 
    }

    private string _newIngredientName = "Enter ingredient";
    public string NewIngredientName
    {
        get => _newIngredientName;
        set
        {
            _newIngredientName = value;
            OnPropertyChanged();
            ApplyIngredientSearchFilter();
        }
    }

    private string _newIngredientQuantity;
    public string NewIngredientQuantity
    {
        get => _newIngredientQuantity;
        set
        {
            _newIngredientQuantity = value;
            OnPropertyChanged();
        }
    }

    private Units _newIngredientUnit;
    public Units NewIngredientUnit
    {
        get => _newIngredientUnit;
        set
        {
            _newIngredientUnit = value;
            OnPropertyChanged();
        }
    }
    public ICommand AddRecipeIngredientCommand { get; }
    public ICommand SaveRecipeCommand { get; }
    public ICommand DeleteRecipeCommand { get; }
    public DetailedRecipeViewModel(GetStaticListDataService staticDataService, IngredientService ingredientService)
    {
        _staticDataService = staticDataService;
        _ingredientService = ingredientService;

        AllIngredients = new ObservableCollection<Ingredients>();
        FilteredIngredients = new ObservableCollection<Ingredients>();

        LoadAllIngredients();

        LoadData();

        if (Recipe == null)
        {
            Recipe = new Model.Recipes
            {
                Recipe = "New Recipe",
                CookingInstructions = string.Empty,
                CookingTimeId = CookingTimes.FirstOrDefault()?.Id ?? 0,
                RecipeIngredients = new List<RecipeIngredients>
            {
                new RecipeIngredients
                {
                    Ingredient = new Ingredients { Ingredient = "Potatis" },
                    Unit = Units.FirstOrDefault(),
                    Quantity = "500"
                }
            },
                RecipeRecipeTags = new List<RecipeRecipeTags>
            {
                new RecipeRecipeTags
                {
                    RecipeTag = RecipeTags.FirstOrDefault()
                }
            }
            };
        }

        AddRecipeIngredientCommand = new RelayCommand(AddRecipeIngredient);
        SaveRecipeCommand = new RelayCommand(SaveRecipe);
        DeleteRecipeCommand = new RelayCommand(DeleteRecipe);
    }

    private async void LoadAllIngredients()
    {
        var ingredients = await _ingredientService.GetAllIngredientsAsync();
        AllIngredients = new ObservableCollection<Ingredients>(ingredients);
        FilterAvailableIngredients();
    }
    private void FilterAvailableIngredients()
    {
        if (Recipe == null || AllIngredients == null) return;

        var existingIngredients = Recipe.RecipeIngredients
            .Select(ri => ri.Ingredient.Ingredient)
            .ToHashSet();

        var availableIngredients = AllIngredients
            .Where(i => !existingIngredients.Contains(i.Ingredient))
            .OrderBy(i => i.Ingredient) 
            .ToList();

        FilteredIngredients.Clear();
        foreach (var ingredient in availableIngredients)
        {
            FilteredIngredients.Add(ingredient);
        }

        ApplyIngredientSearchFilter();
    }

    private void ApplyIngredientSearchFilter()
    {
        if (string.IsNullOrWhiteSpace(NewIngredientName) || NewIngredientName == "Enter ingredient")
        {
            FilteredIngredients.Clear();
            foreach (var ingredient in AllIngredients.OrderBy(i => i.Ingredient))
            {
                FilteredIngredients.Add(ingredient);
            }
        }
        else
        {
            var matchingIngredients = AllIngredients
                .Where(i => i.Ingredient.IndexOf(NewIngredientName, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(i => i.Ingredient)
                .ToList();

            FilteredIngredients.Clear();
            foreach (var ingredient in matchingIngredients)
            {
                FilteredIngredients.Add(ingredient);
            }
        }
    }
    private void LoadData()
    {
        Units = new ObservableCollection<Units>(_staticDataService.GetUnits());
        CookingTimes = new ObservableCollection<CookingTimes>(_staticDataService.GetCookingTimes());
        RecipeTags = new ObservableCollection<RecipeTags>(_staticDataService.GetRecipeTags());
    }
    private void AddRecipeIngredient(object obj)
    {
        if (string.IsNullOrWhiteSpace(NewIngredientName) || NewIngredientQuantity == null || NewIngredientUnit == null)
        {
            MessageBox.Show("Please provide valid values for all fields before adding an ingredient.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var newIngredient = new RecipeIngredients
        {
            Ingredient = new Ingredients { Ingredient = NewIngredientName },
            Quantity = NewIngredientQuantity,
            Unit = NewIngredientUnit
        };

        Recipe.RecipeIngredients.Add(newIngredient);

        NewIngredientName = string.Empty;
        NewIngredientQuantity = string.Empty;
        NewIngredientUnit = null;

        OnPropertyChanged(nameof(Recipe));
        FilterAvailableIngredients();
    }

    private void SaveRecipe(object obj)
    {
        // if not exists in database post, else push updates
    }
    private void DeleteRecipe(object obj)
    {
        // DELETE from database
        var result = MessageBox.Show("Are you sure you want to delete this recipe?", "Confirm Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            // Code to delete the recipe from the database
            throw new NotImplementedException();
        }
        // Continue without deleting
    }
    

}
