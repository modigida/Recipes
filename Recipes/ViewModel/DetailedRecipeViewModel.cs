﻿using Recipes.Commands;
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

    private bool _isApplyingFilter = false;
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

    private bool _isDeleteAvailable;
    public bool IsDeleteAvailable
    {
        get => _isDeleteAvailable;
        set
        {
            _isDeleteAvailable = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSaveAvailable));
        }
    }

    private bool _isSaveAvailable;
    public bool IsSaveAvailable
    {
        get => _isSaveAvailable;
        set
        {
            _isSaveAvailable = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsDeleteAvailable));
        }
    }

    private bool _isFavoriteRecipe;
    public bool IsFavoriteRecipe
    {
        get => _isFavoriteRecipe;
        set
        {
            _isFavoriteRecipe = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotFavoriteRecipe));
        }
    }

    private bool _isNotFavoriteRecipe;
    public bool IsNotFavoriteRecipe
    {
        get => _isNotFavoriteRecipe;
        set
        {
            _isNotFavoriteRecipe = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsFavoriteRecipe));
        }
    }

    private string _newIngredientName;
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
            NewIngredientName = _selectedRecipeIngredient?.Ingredient?.Ingredient.ToString() ?? string.Empty;
            NewIngredientQuantity = (double)(_selectedRecipeIngredient?.Quantity ?? 0);

            if (_selectedRecipeIngredient != null)
            {
                NewIngredientUnit = Units.FirstOrDefault(u => u.Id == _selectedRecipeIngredient.UnitId) ?? Units.FirstOrDefault(u => u.Id == 8);
            }
            else
            {
                NewIngredientUnit = Units?.FirstOrDefault(u => u.Id == 8);
            }

            OnPropertyChanged();

        }
    }

    private double? _newIngredientQuantity;
    public double? NewIngredientQuantity
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
            OnPropertyChanged();
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

        AllIngredients = new ObservableCollection<Ingredients>();
        FilteredIngredients = new ObservableCollection<Ingredients>();

        LoadAllIngredients();

        LoadData();

        ShowRecipeViewCommand = new RelayCommand(ShowRecipeView);
        AddRecipeIngredientCommand = new RelayCommand(AddRecipeIngredient);
        DeleteRecipeIngredientCommand = new RelayCommand<RecipeIngredients>(async recipeIngredient => await DeleteRecipeIngredient(recipeIngredient));
        CreateRecipeCommand = new RelayCommand(CreateRecipe);
        DeleteRecipeCommand = new RelayCommand(DeleteRecipe);
        IsFavoriteCommand = new RelayCommand(IsFavorite);
    }


    private async void LoadAllIngredients()
    {
        var ingredients = await _ingredientService.GetAllIngredientsAsync();
        AllIngredients = new ObservableCollection<Ingredients>(ingredients);
    }

    private async void IsFavorite(object obj)
    {
        if (Recipe.IsFavorite)
        {
            Recipe.IsFavorite = false;
            IsFavoriteRecipe = false;
            IsNotFavoriteRecipe = true;
        }
        else
        {
            Recipe.IsFavorite = true;
            IsFavoriteRecipe = true;
            IsNotFavoriteRecipe = false;
        }

        OnPropertyChanged(nameof(Recipe.IsFavorite));
        OnPropertyChanged(nameof(IsFavoriteRecipe));
        OnPropertyChanged(nameof(IsNotFavoriteRecipe));
        await UpdateRecipe();
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
        if (_isApplyingFilter) return;

        _isApplyingFilter = true;

        try
        {
            if (string.IsNullOrWhiteSpace(NewIngredientName) || NewIngredientName == "Enter ingredient")
            {
                FilteredIngredients.Clear();
                Console.WriteLine(AllIngredients.Count);
                ShowSuggestions = false;
            }
            else
            {
                var bestMatch = AllIngredients
                    .Where(i => i.Ingredient.StartsWith(NewIngredientName, StringComparison.OrdinalIgnoreCase) &&
                                !RecipeRecipeIngredients.Any(ri => ri.Ingredient.Id == i.Id))
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
                    ShowSuggestions = false;
                }
            }
        }
        finally
        {
            _isApplyingFilter = false;
        }
    }
    public async Task LoadData(Model.Recipes recipe = null)
    {
        Units = new ObservableCollection<Units>(_staticDataService.GetUnits());
        CookingTimes = new ObservableCollection<CookingTimes>(_staticDataService.GetCookingTimes());
        RecipeTags = new ObservableCollection<RecipeTags>(_staticDataService.GetRecipeTags());

        if (recipe != null)
        {
            Recipe = await _recipeService.GetRecipeByIdAsync(recipe.Id);
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
        NewIngredientName = "Enter ingredient";
        NewIngredientQuantity = 0;

        if (Recipe.Id != 0)
        {
            IsDeleteAvailable = true;
            IsSaveAvailable = false;
        }
        else
        {
            IsDeleteAvailable = false;
            IsSaveAvailable = true;
        }
        if (Recipe.IsFavorite) 
        {
            IsFavoriteRecipe = true;
            IsNotFavoriteRecipe = false;
        }
        else
        {
            IsFavoriteRecipe = false;
            IsNotFavoriteRecipe = true;
        }

        await LoadTags();
        await SortRecipeIngredients();

        FilterAvailableIngredients();
    }

    private async Task LoadTags()
    {
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

    private async void AddRecipeIngredient(object obj)
    {
        if (string.IsNullOrWhiteSpace(NewIngredientName) || NewIngredientName == "Enter ingredient" || NewIngredientQuantity == null || NewIngredientUnit == null)
        {
            MessageBox.Show("Please provide valid values for all fields before adding an ingredient.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var newIngredient = new RecipeIngredients
        {
            Ingredient = new Ingredients { Ingredient = NewIngredientName },
            Quantity = (double)NewIngredientQuantity,
            UnitId = NewIngredientUnit.Id,
            Unit = NewIngredientUnit
        };

        if (RecipeRecipeIngredients.Any(ri => ri.Ingredient.Ingredient == newIngredient.Ingredient.Ingredient))
        {

            var existingRecipeIngredient = RecipeRecipeIngredients
                .First(ri => ri.Ingredient.Ingredient == newIngredient.Ingredient.Ingredient);

            RecipeRecipeIngredients.Remove(existingRecipeIngredient);

            existingRecipeIngredient.Quantity = newIngredient.Quantity;
            existingRecipeIngredient.Unit = newIngredient.Unit;

            RecipeRecipeIngredients.Add(existingRecipeIngredient);

            if (Recipe.Id != 0)
            {
                await _recipeIngredientService.UpdateRecipeIngredientAsync(
                    Recipe.Id,
                    existingRecipeIngredient.Ingredient.Id,
                    existingRecipeIngredient.Quantity,
                    existingRecipeIngredient.Unit.Id);
            }

            OnPropertyChanged(nameof(RecipeRecipeIngredients));
            FilterAvailableIngredients();
        }
        else
        {
            RecipeRecipeIngredients.Add(newIngredient);

            if (Recipe.Id != 0)
            {
                var existingIngredient = await _ingredientService.GetIngredientByNameAsync(newIngredient.Ingredient.Ingredient);

                if (existingIngredient == null)
                {
                    newIngredient.Ingredient.Id = await _ingredientService.AddIngredientAsync(newIngredient.Ingredient);
                }
                else
                {
                    newIngredient.Ingredient = existingIngredient;
                    _ingredientService.AttachIngredient(newIngredient.Ingredient);
                }

                await _recipeIngredientService.AddRecipeIngredientAsync(
                    Recipe.Id,
                    newIngredient.Ingredient.Id,
                    newIngredient.Quantity,
                    newIngredient.Unit.Id);
            }

            FilterAvailableIngredients();
        }

        await SortRecipeIngredients();

        NewIngredientName = "Enter ingredient";
        NewIngredientQuantity = 0;
        NewIngredientUnit = Units.FirstOrDefault(u => u.Id == 8); 
    }

    private async Task SortRecipeIngredients()
    {
        var sortedList = RecipeRecipeIngredients
        .OrderBy(ri => ri.Ingredient.Ingredient)
        .ToList();

        RecipeRecipeIngredients.Clear();
        foreach (var ingredient in sortedList)
        {
            RecipeRecipeIngredients.Add(ingredient);
        }
    }

    private async Task DeleteRecipeIngredient(RecipeIngredients recipeIngredient)
    {
        if (recipeIngredient == null)
        {
            MessageBox.Show("Please select an ingredient to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirmResult = MessageBox.Show($"Are you sure you want to remove '{recipeIngredient.Ingredient.Ingredient}' from the recipe?",
                                            "Confirm Delete",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Question);

        if (confirmResult != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _recipeIngredientService.RemoveRecipeIngredientAsync(Recipe.Id, recipeIngredient.Ingredient.Id);

            RecipeRecipeIngredients.Remove(recipeIngredient);
            Recipe.RecipeIngredients.Remove(recipeIngredient);

            SelectedRecipeIngredient = null;

            OnPropertyChanged(nameof(Recipe));
            OnPropertyChanged(nameof(RecipeRecipeIngredients));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to remove ingredient: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
        else
        {
            Recipe.CookingTimeId = SelectedCookingTime.Id;
            await _recipeService.UpdateRecipeAsync(Recipe);
        }

        await SaveTags();

        if (Recipe.Id != 0)
        {

            var dbRecipeIngredients = await _recipeIngredientService.GetIngredientsByRecipeIdAsync(Recipe.Id);
            
            foreach (var recipeIngredient in RecipeRecipeIngredients)
            {
                var dbRecipeIngredient = dbRecipeIngredients
                    .FirstOrDefault(ri => ri.Ingredient.Id == recipeIngredient.Ingredient.Id);
            
                if (dbRecipeIngredient == null)
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
                else
                {
                    if (dbRecipeIngredient.Quantity != recipeIngredient.Quantity ||
                        dbRecipeIngredient.Unit.Id != recipeIngredient.Unit.Id)
                    {
                        await _recipeIngredientService.UpdateRecipeIngredientAsync(
                            Recipe.Id,
                            recipeIngredient.Ingredient.Id,
                            recipeIngredient.Quantity,
                            recipeIngredient.Unit.Id);
                    }
                }
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
