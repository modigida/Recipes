using Microsoft.EntityFrameworkCore;
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
    private readonly TagService _tagService;
    private readonly RecipeService _recipeService;
    private readonly RecipeIngredientService _recipeIngredientService;
    private readonly RecipeViewModel _recipeViewModel;
    private readonly MainWindowViewModel _mainWindowViewModel;

    public ObservableCollection<Ingredients> AllIngredients { get; set; }
    public ObservableCollection<Ingredients> FilteredIngredients { get; set; }
    public ObservableCollection<Units> Units { get; set; }
    public static ObservableCollection<CookingTimes> CookingTimes { get; set; }
    public ObservableCollection<RecipeTags> RecipeTags { get; set; }


    private ObservableCollection<RecipeIngredients> _recipeRecipeIngredients;
    public ObservableCollection<RecipeIngredients> RecipeRecipeIngredients 
    { 
        get => _recipeRecipeIngredients; 
        set
        {
            _recipeRecipeIngredients = value;
            OnPropertyChanged();
        }  
    }


    private ObservableCollection<RecipeIngredients> _newRecipeIngredients;
    public ObservableCollection<RecipeIngredients> NewRecipeIngredients
    {
        get => _newRecipeIngredients;
        set
        {
            _newRecipeIngredients = value;
            OnPropertyChanged();
        }
    }


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

    private Ingredients _selectedIngredient;
    public Ingredients SelectedIngredient
    {
        get => _selectedIngredient;
        set
        {
            _selectedIngredient = value;
            OnPropertyChanged();
            NewIngredientName = _selectedIngredient?.Ingredient ?? string.Empty;
            ShowSuggestions = false;
        }
    }

    private RecipeIngredients _selectedRecipeIngredient;
    public RecipeIngredients SelectedRecipeIngredient
    {
        get => _selectedRecipeIngredient;
        set
        {
            _selectedRecipeIngredient = value;
            OnPropertyChanged();
            NewIngredientName = _selectedRecipeIngredient?.Ingredient?.Ingredient.ToString() ?? string.Empty;
            NewIngredientQuantity = _selectedRecipeIngredient?.Quantity?.ToString() ?? string.Empty;
            NewIngredientUnit = _selectedRecipeIngredient?.Unit ?? Units[8];
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

    private CookingTimes _selectedCookingTime;

    public CookingTimes SelectedCookingTime
    {
        get => _selectedCookingTime;
        set
        {
            _selectedCookingTime = value;
            OnPropertyChanged(nameof(SelectedCookingTime));
        }
    }


    private bool _showSuggestions;
    public bool ShowSuggestions
    {
        get => _showSuggestions;
        set
        {
            _showSuggestions = value;
            OnPropertyChanged();
        }
    }


    public ICommand AddRecipeIngredientCommand { get; }
    public ICommand SaveRecipeCommand { get; }
    public ICommand DeleteRecipeCommand { get; }
    public DetailedRecipeViewModel(GetStaticListDataService staticDataService,
        IngredientService ingredientService, TagService tagsService, RecipeService recipeService,
        RecipeIngredientService recipeIngredientService, RecipeViewModel recipeViewModel,
        MainWindowViewModel mainWindowViewModel)
    {
        _staticDataService = staticDataService;
        _ingredientService = ingredientService;
        _tagService = tagsService;
        _recipeService = recipeService;
        _recipeIngredientService = recipeIngredientService;
        _recipeViewModel = recipeViewModel;
        _mainWindowViewModel = mainWindowViewModel;

        AllIngredients = new ObservableCollection<Ingredients>();
        FilteredIngredients = new ObservableCollection<Ingredients>();
        NewRecipeIngredients = new ObservableCollection<RecipeIngredients>();

        LoadAllIngredients();

        LoadData();

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

    private bool _isApplyingFilter = false;
    private void ApplyIngredientSearchFilter()
    {
        if (_isApplyingFilter) return;

        _isApplyingFilter = true;

        try
        {
            if (string.IsNullOrWhiteSpace(NewIngredientName) || NewIngredientName == "Enter ingredient")
            {
                FilteredIngredients.Clear();
                FilteredIngredients.Add(new Ingredients { Ingredient = " " });
                ShowSuggestions = false;
            }
            else
            {
                var bestMatch = AllIngredients
                    .Where(i => i.Ingredient.StartsWith(NewIngredientName, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(i => i.Ingredient)
                    .FirstOrDefault();

                FilteredIngredients.Clear();

                if (bestMatch != null)
                {
                    FilteredIngredients.Add(bestMatch);
                    ShowSuggestions = true;

                    if (SelectedRecipeIngredient != null && bestMatch.Ingredient.ToString() == SelectedRecipeIngredient.Ingredient.Ingredient.ToString())
                    {
                        ShowSuggestions = false;
                    }
                }
                else
                {
                    FilteredIngredients.Add(new Ingredients { Ingredient = " " });
                    ShowSuggestions = false;
                }
            }
        }
        finally
        {
            _isApplyingFilter = false;
        }
    }
    public async void LoadData(Model.Recipes recipe = null)
    {
        Units = new ObservableCollection<Units>(_staticDataService.GetUnits());
        CookingTimes = new ObservableCollection<CookingTimes>(_staticDataService.GetCookingTimes());
        RecipeTags = new ObservableCollection<RecipeTags>(_staticDataService.GetRecipeTags());

        if (recipe != null)
        {
            Recipe = recipe;
        }
        else
        {
            Recipe = new Model.Recipes
            {
                Recipe = "New Recipe",
                CookingInstructions = string.Empty,
                CookingTimeId = CookingTimes.FirstOrDefault(ct => ct.Id == 2)?.Id ?? 0,
                RecipeIngredients = new ObservableCollection<RecipeIngredients>()
            };
        }

        RecipeRecipeIngredients = new ObservableCollection<RecipeIngredients>(Recipe.RecipeIngredients);
        SelectedCookingTime = CookingTimes.FirstOrDefault(ct => ct.Id == Recipe.CookingTimeId);
        NewIngredientUnit = Units.FirstOrDefault(u => u.Id == 8);

        if (Recipe?.Id > 0)
        {
            var tagsForRecipe = await _tagService.GetTagsForRecipeAsync(Recipe.Id);

            foreach (var tag in RecipeTags)
            {
                tag.IsSelected = tagsForRecipe.Contains(tag.Id);
            }

            OnPropertyChanged(nameof(RecipeTags));
        }
        else
        {
            foreach (var tag in RecipeTags)
            {
                tag.IsSelected = false;
            }

            OnPropertyChanged(nameof(RecipeTags));
        }
    }
    private void AddRecipeIngredient(object obj)
    {
        if (string.IsNullOrWhiteSpace(NewIngredientName) || string.IsNullOrWhiteSpace(NewIngredientQuantity) || NewIngredientUnit == null)
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

        //Recipe.RecipeIngredients.Add(newIngredient);
        RecipeRecipeIngredients.Add(newIngredient);

        if (!NewRecipeIngredients.Any(ri => ri.Ingredient.Ingredient == newIngredient.Ingredient.Ingredient))
        {
            NewRecipeIngredients.Add(newIngredient);
            OnPropertyChanged(nameof(Recipe.RecipeIngredients));
        }

        NewIngredientName = string.Empty;
        NewIngredientQuantity = string.Empty;
        NewIngredientUnit = null;

        OnPropertyChanged(nameof(Recipe));
        FilterAvailableIngredients();
    }

    private async void SaveRecipe(object obj)
    {
        if (Recipe.Id == 0)
        {
            Recipe.Id = await _recipeService.AddRecipeAsync(Recipe);
        }
        else
        {
            Recipe.CookingTimeId = SelectedCookingTime.Id;
            await _recipeService.UpdateRecipeAsync(Recipe);
        }

        var selectedTagIds = RecipeTags
            .Where(tag => (bool)tag.IsSelected)
            .Select(tag => tag.Id)
            .ToList();

        await _tagService.SaveSelectedTagsAsync(Recipe.Id, selectedTagIds);

        if (Recipe.Id != 0)
        {
            foreach (var recipeIngredient in NewRecipeIngredients)
            {
                var existingIngredient = await _ingredientService.GetIngredientByNameAsync(recipeIngredient.Ingredient.Ingredient);

                if (existingIngredient == null)
                {
                    recipeIngredient.Ingredient.Id = await _ingredientService.AddIngredientAsync(recipeIngredient.Ingredient);
                }
                else
                {
                    recipeIngredient.Ingredient = existingIngredient;
                    _ingredientService.AttachIngredient(recipeIngredient.Ingredient);
                }

                await _recipeIngredientService.AddRecipeIngredientAsync(
                    Recipe.Id,
                    recipeIngredient.Ingredient.Id,
                    recipeIngredient.Quantity,
                    recipeIngredient.Unit.Id);
            }
        }

        NewRecipeIngredients.Clear();

        MessageBox.Show("Recipe saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);

        _mainWindowViewModel.ShowRecipeViewCommand.Execute(null);
    }

    private async void DeleteRecipe(object obj)
    {
        var result = MessageBox.Show("Are you sure you want to delete this recipe?", "Confirm Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            await _recipeService.DeleteRecipeAsync(Recipe.Id);
            MessageBox.Show("Recipe deleted successfully.", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
            _recipeViewModel.LoadRecipes();
            _mainWindowViewModel.ShowRecipeViewCommand?.Execute(null);
        }
    }
}
