using Recipes.Commands;
using Recipes.Model;
using Recipes.Services;
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

    private DetailedRecipeIngredientViewModel _detailedRecipeIngredientViewModel;
    private DetailedRecipeLoadDataViewModel _detailedRecipeLoadDataViewModel;

    public ObservableCollection<Ingredients> AllIngredients { get; set; }
    public ObservableCollection<Ingredients> FilteredIngredients { get; set; }
    public static ObservableCollection<Units> Units { get; set; } = new();
    public static ObservableCollection<CookingTimes> CookingTimes { get; set; } = new();
    public ObservableCollection<RecipeTags> RecipeTags { get; set; }

    private ObservableCollection<RecipeIngredients> _recipeRecipeIngredients;
    public ObservableCollection<RecipeIngredients> RecipeRecipeIngredients
    {
        get => _recipeRecipeIngredients;
        set => SetProperty(ref _recipeRecipeIngredients, value);
    }

    private Model.Recipes _recipe;
    public Model.Recipes Recipe
    {
        get => _recipe;
        set => SetProperty(ref _recipe, value);
    }

    private bool _isDeleteAvailable;
    public bool IsDeleteAvailable
    {
        get => _isDeleteAvailable;
        set => SetProperty(ref _isDeleteAvailable, value, new[] { nameof(IsSaveAvailable) });
    }

    private bool _isSaveAvailable;
    public bool IsSaveAvailable
    {
        get => _isSaveAvailable;
        set => SetProperty(ref _isSaveAvailable, value);
    }


    private bool _isFavoriteRecipe;
    public bool IsFavoriteRecipe
    {
        get => _isFavoriteRecipe;
        set => SetProperty(ref _isFavoriteRecipe, value, new[] { nameof(IsNotFavoriteRecipe) });
    }

    private bool _isNotFavoriteRecipe;
    public bool IsNotFavoriteRecipe
    {
        get => _isNotFavoriteRecipe;
        set => SetProperty(ref _isNotFavoriteRecipe, value, new[] { nameof(IsFavoriteRecipe) });
    }

    private string _newIngredientName;
    public string NewIngredientName
    {
        get => _newIngredientName;
        set
        {
            if (SetProperty(ref _newIngredientName, value))
            {
                _detailedRecipeIngredientViewModel.ApplyIngredientSearchFilter();
            }
        }
    }

    private Ingredients _selectedIngredient;
    public Ingredients SelectedIngredient
    {
        get => _selectedIngredient;
        set
        {
            if (SetProperty(ref _selectedIngredient, value))
            {
                NewIngredientName = _selectedIngredient?.Ingredient ?? string.Empty;
                ShowSuggestions = false;
            }
        }
    }

    private RecipeIngredients _selectedRecipeIngredient;
    public RecipeIngredients SelectedRecipeIngredient
    {
        get => _selectedRecipeIngredient;
        set
        {
            if (SetProperty(ref _selectedRecipeIngredient, value))
            {
                NewIngredientName = _selectedRecipeIngredient?.Ingredient?.Ingredient.ToString() ?? string.Empty;
                NewIngredientQuantity = (double)(_selectedRecipeIngredient?.Quantity ?? 0);

                NewIngredientUnit = _selectedRecipeIngredient != null
                    ? Units.FirstOrDefault(u => u.Id == _selectedRecipeIngredient.UnitId) ?? Units.FirstOrDefault(u => u.Id == 8)
                    : Units?.FirstOrDefault(u => u.Id == 8);
            }
        }
    }

    private double? _newIngredientQuantity;
    public double? NewIngredientQuantity
    {
        get => _newIngredientQuantity;
        set => SetProperty(ref _newIngredientQuantity, value);
    }

    private Units _newIngredientUnit;
    public Units NewIngredientUnit
    {
        get => _newIngredientUnit;
        set => SetProperty(ref _newIngredientUnit, value);
    }

    private CookingTimes _selectedCookingTime;
    public CookingTimes SelectedCookingTime
    {
        get => _selectedCookingTime;
        set => SetProperty(ref _selectedCookingTime, value);
    }

    private bool _showSuggestions;
    public bool ShowSuggestions
    {
        get => _showSuggestions;
        set => SetProperty(ref _showSuggestions, value);
    }
    public ICommand ShowRecipeViewCommand { get; }
    public ICommand AddRecipeIngredientCommand { get; }
    public ICommand DeleteRecipeIngredientCommand { get; }
    public ICommand CreateRecipeCommand { get; }
    public ICommand DeleteRecipeCommand { get; }
    public ICommand IsFavoriteCommand { get; }
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

        _detailedRecipeIngredientViewModel = new(this, ingredientService, recipeIngredientService);
        _detailedRecipeLoadDataViewModel = new(this);

        AllIngredients = new ObservableCollection<Ingredients>();
        FilteredIngredients = new ObservableCollection<Ingredients>();

        _detailedRecipeLoadDataViewModel.LoadAllIngredients(ingredientService);

        LoadData();

        ShowRecipeViewCommand = new RelayCommand(ShowRecipeView);
        AddRecipeIngredientCommand = new RelayCommand(_detailedRecipeIngredientViewModel.AddRecipeIngredient);
        DeleteRecipeIngredientCommand = new RelayCommand<RecipeIngredients>(async recipeIngredient => await _detailedRecipeIngredientViewModel.DeleteRecipeIngredient(recipeIngredient));
        CreateRecipeCommand = new RelayCommand(CreateRecipe);
        DeleteRecipeCommand = new RelayCommand(DeleteRecipe);
        IsFavoriteCommand = new RelayCommand(IsFavorite);
    }
    private async void IsFavorite(object obj)
    {
        Recipe.IsFavorite = !Recipe.IsFavorite;
        IsFavoriteRecipe = Recipe.IsFavorite;
        IsNotFavoriteRecipe = !Recipe.IsFavorite;

        OnPropertyChanged(nameof(Recipe.IsFavorite));
        OnPropertyChanged(nameof(IsFavoriteRecipe));
        OnPropertyChanged(nameof(IsNotFavoriteRecipe));
        await UpdateRecipe();
    }
    public async Task LoadData(Model.Recipes recipe = null)
    {
        _detailedRecipeLoadDataViewModel.LoadAllLists(_staticDataService);

        Recipe = await _detailedRecipeLoadDataViewModel.LoadRecipe(recipe?.Id ?? 0, _recipeService);

        UpdateUI();

        SetDeleteStatus();
        SetFavoriteStatus();

        var updatedTags = await _detailedRecipeLoadDataViewModel.LoadTags(RecipeTags.ToList(), _tagService, recipe);
        RecipeTags = new ObservableCollection<RecipeTags>(updatedTags);
        OnPropertyChanged(nameof(RecipeTags));

        _detailedRecipeIngredientViewModel.SortRecipeIngredients();

        _detailedRecipeIngredientViewModel.FilterAvailableIngredients();
    }
    private void UpdateUI()
    {
        RecipeRecipeIngredients = new ObservableCollection<RecipeIngredients>(Recipe.RecipeIngredients);
        SelectedCookingTime = CookingTimes.FirstOrDefault(ct => ct.Id == Recipe.CookingTimeId);
        NewIngredientUnit = Units.FirstOrDefault(u => u.Id == 8);
        NewIngredientName = "Enter ingredient";
        NewIngredientQuantity = 0;
    }
    private void SetDeleteStatus()
    {
        IsDeleteAvailable = Recipe.Id != 0;
        IsSaveAvailable = !IsDeleteAvailable;
    }
    private void SetFavoriteStatus()
    {
        IsFavoriteRecipe = Recipe.IsFavorite;
        IsNotFavoriteRecipe = !IsFavoriteRecipe;
    }
    private async void ShowRecipeView(object obj)
    {
        if (Recipe.Id != 0)
        {
            await UpdateRecipe();
            await SaveTags();
        }
        else
        {
            if (Recipe.Recipe != "New Recipe" || RecipeRecipeIngredients.Count > 0)
            {
                var result = MessageBox.Show("You have not saved the recipe. If you continue without saving, all changes will be lost. " +
                                "Do you want to save before proceeding?", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    CreateRecipe(null);
                }
            }
        }
        _mainWindowViewModel.ShowRecipeViewCommand.Execute(null);
    }
    private async Task UpdateRecipe()
    {
        if (_recipe.Id != 0)
        {
            Recipe.CookingTimeId = SelectedCookingTime.Id;
            await _recipeService.UpdateRecipeAsync(Recipe);
        }
    }
    private async Task SaveTags()
    {
        var selectedTagIds = RecipeTags
            .Where(tag => (bool)tag.IsSelected)
            .Select(tag => tag.Id)
            .ToList();

        await _tagService.SaveSelectedTagsAsync(Recipe.Id, selectedTagIds);
    }
    private async void CreateRecipe(object obj)
    {
        if (Recipe.Id == 0)
        {
            Recipe.Id = await _recipeService.AddRecipeAsync(Recipe);
        }

        await SaveTags();

        if (Recipe.Id != 0)
        {
            foreach (var recipeIngredient in RecipeRecipeIngredients)
            {
                await _detailedRecipeIngredientViewModel.HandleNewIngredientAsync(recipeIngredient);
            }
        }
        MessageBox.Show("Recipe saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
        ShowRecipeView(null);
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