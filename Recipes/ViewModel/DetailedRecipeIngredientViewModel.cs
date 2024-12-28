using Recipes.Model;
using Recipes.Services;
using System.Windows;

namespace Recipes.ViewModel;
public class DetailedRecipeIngredientViewModel : BaseViewModel
{
    private readonly IngredientService _ingredientService;
    private readonly RecipeIngredientService _recipeIngredientService;
    private readonly DetailedRecipeViewModel _detailedRecipeViewModel;
    private bool _isApplyingFilter = false;

    public DetailedRecipeIngredientViewModel(DetailedRecipeViewModel detailedRecipeViewModel,
        IngredientService ingredientService, RecipeIngredientService recipeIngredientService)
    {
        _detailedRecipeViewModel = detailedRecipeViewModel;
        _ingredientService = ingredientService;
        _recipeIngredientService = recipeIngredientService;
    }
    public async void AddRecipeIngredient(object obj)
    {
        if (!IsValidNewIngredientInput())
        {
            MessageBox.Show("Please provide valid values for all fields before adding an ingredient.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var newIngredient = CreateNewRecipeIngredient();

        if (IsIngredientAlreadyInRecipe(newIngredient))
        {
            await UpdateExistingRecipeIngredientAsync(newIngredient);
        }
        else
        {
            await AddNewRecipeIngredientAsync(newIngredient);
        }

        FinalizeIngredientAddition();
    }
    private bool IsValidNewIngredientInput()
    {
        return !string.IsNullOrWhiteSpace(_detailedRecipeViewModel.NewIngredientName) &&
               _detailedRecipeViewModel.NewIngredientName != "Enter ingredient" &&
               _detailedRecipeViewModel.NewIngredientQuantity != null &&
               _detailedRecipeViewModel.NewIngredientUnit != null;
    }
    private RecipeIngredients CreateNewRecipeIngredient()
    {
        return new RecipeIngredients
        {
            Ingredient = new Ingredients { Ingredient = _detailedRecipeViewModel.NewIngredientName },
            Quantity = (double)_detailedRecipeViewModel.NewIngredientQuantity,
            UnitId = _detailedRecipeViewModel.NewIngredientUnit.Id,
            Unit = _detailedRecipeViewModel.NewIngredientUnit
        };
    }
    private bool IsIngredientAlreadyInRecipe(RecipeIngredients newIngredient)
    {
        return _detailedRecipeViewModel.RecipeRecipeIngredients
            .Any(ri => ri.Ingredient.Ingredient == newIngredient.Ingredient.Ingredient);
    }
    private async Task AddNewRecipeIngredientAsync(RecipeIngredients newIngredient)
    {
        _detailedRecipeViewModel.RecipeRecipeIngredients.Add(newIngredient);

        if (_detailedRecipeViewModel.Recipe.Id != 0)
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
                _detailedRecipeViewModel.Recipe.Id,
                newIngredient.Ingredient.Id,
                newIngredient.Quantity,
                newIngredient.Unit.Id);
        }

        FilterAvailableIngredients();
    }
    private void FinalizeIngredientAddition()
    {
        SortRecipeIngredients();

        _detailedRecipeViewModel.NewIngredientName = "Enter ingredient";
        _detailedRecipeViewModel.NewIngredientQuantity = 0;
        _detailedRecipeViewModel.NewIngredientUnit = DetailedRecipeViewModel.Units.FirstOrDefault(u => u.Id == 8);
    }
    public void FilterAvailableIngredients()
    {
        if (_detailedRecipeViewModel.Recipe == null || _detailedRecipeViewModel.AllIngredients == null) return;

        var existingIngredients = _detailedRecipeViewModel.Recipe.RecipeIngredients
            .Select(ri => ri.Ingredient.Ingredient)
            .ToHashSet();

        var availableIngredients = _detailedRecipeViewModel.AllIngredients
            .Where(i => !existingIngredients.Contains(i.Ingredient))
            .OrderBy(i => i.Ingredient)
            .ToList();

        _detailedRecipeViewModel.FilteredIngredients.Clear();
        foreach (var ingredient in availableIngredients)
        {
            _detailedRecipeViewModel.FilteredIngredients.Add(ingredient);
        }

        ApplyIngredientSearchFilter();
    }
    public void ApplyIngredientSearchFilter()
    {
        if (_isApplyingFilter) return;

        _isApplyingFilter = true;

        try
        {
            if (string.IsNullOrWhiteSpace(_detailedRecipeViewModel.NewIngredientName) || _detailedRecipeViewModel.NewIngredientName == "Enter ingredient")
            {
                _detailedRecipeViewModel.FilteredIngredients.Clear();
                Console.WriteLine(_detailedRecipeViewModel.AllIngredients.Count);
                _detailedRecipeViewModel.ShowSuggestions = false;
            }
            else
            {
                var bestMatch = _detailedRecipeViewModel.AllIngredients
                    .Where(i => i.Ingredient.Contains(_detailedRecipeViewModel.NewIngredientName, StringComparison.OrdinalIgnoreCase) &&
                                !_detailedRecipeViewModel.RecipeRecipeIngredients.Any(ri => ri.Ingredient.Id == i.Id))
                    .OrderBy(i => i.Ingredient)
                    .FirstOrDefault();

                _detailedRecipeViewModel.FilteredIngredients.Clear();

                if (bestMatch != null)
                {
                    _detailedRecipeViewModel.FilteredIngredients.Add(bestMatch);
                    _detailedRecipeViewModel.ShowSuggestions = true;

                    if (_detailedRecipeViewModel.SelectedRecipeIngredient != null && bestMatch.Ingredient.ToString() == _detailedRecipeViewModel.SelectedRecipeIngredient.Ingredient.Ingredient.ToString())
                    {
                        _detailedRecipeViewModel.ShowSuggestions = false;
                    }
                }
                else
                {
                    _detailedRecipeViewModel.ShowSuggestions = false;
                }
            }
        }
        finally
        {
            _isApplyingFilter = false;
        }
    }
    public void SortRecipeIngredients()
    {
        var sortedList = _detailedRecipeViewModel.RecipeRecipeIngredients
        .OrderBy(ri => ri.Ingredient.Ingredient)
        .ToList();

        _detailedRecipeViewModel.RecipeRecipeIngredients.Clear();
        foreach (var ingredient in sortedList)
        {
            _detailedRecipeViewModel.RecipeRecipeIngredients.Add(ingredient);
        }
    }
    public async Task HandleNewIngredientAsync(RecipeIngredients recipeIngredient)
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
            _detailedRecipeViewModel.Recipe.Id,
            recipeIngredient.Ingredient.Id,
            recipeIngredient.Quantity,
            recipeIngredient.Unit.Id);
    }
    private async Task UpdateExistingRecipeIngredientAsync(RecipeIngredients newIngredient)
    {
        var existingRecipeIngredient = _detailedRecipeViewModel.RecipeRecipeIngredients
            .First(ri => ri.Ingredient.Ingredient == newIngredient.Ingredient.Ingredient);

        _detailedRecipeViewModel.RecipeRecipeIngredients.Remove(existingRecipeIngredient);

        existingRecipeIngredient.Quantity = newIngredient.Quantity;
        existingRecipeIngredient.Unit = newIngredient.Unit;
        existingRecipeIngredient.UnitId = newIngredient.UnitId;

        _detailedRecipeViewModel.RecipeRecipeIngredients.Add(existingRecipeIngredient);

        if (_detailedRecipeViewModel.Recipe.Id != 0)
        {
            await _recipeIngredientService.UpdateRecipeIngredientAsync(
                _detailedRecipeViewModel.Recipe.Id,
                existingRecipeIngredient.Ingredient.Id,
                existingRecipeIngredient.Quantity,
                existingRecipeIngredient.Unit.Id);
        }

        OnPropertyChanged(nameof(_detailedRecipeViewModel.RecipeRecipeIngredients));
        FilterAvailableIngredients();
    }
    public async Task DeleteRecipeIngredient(RecipeIngredients recipeIngredient)
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
            await _recipeIngredientService.RemoveRecipeIngredientAsync(_detailedRecipeViewModel.Recipe.Id, recipeIngredient.Ingredient.Id);

            _detailedRecipeViewModel.RecipeRecipeIngredients.Remove(recipeIngredient);
            _detailedRecipeViewModel.Recipe.RecipeIngredients.Remove(recipeIngredient);

            _detailedRecipeViewModel.SelectedRecipeIngredient = null;

            OnPropertyChanged(nameof(_detailedRecipeViewModel.Recipe));
            OnPropertyChanged(nameof(_detailedRecipeViewModel.RecipeRecipeIngredients));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to remove ingredient: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
