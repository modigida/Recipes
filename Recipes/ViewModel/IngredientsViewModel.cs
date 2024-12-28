using Recipes.Commands;
using Recipes.Model;
using Recipes.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace Recipes.ViewModel;
public class IngredientsViewModel : BaseViewModel
{
    private readonly IngredientService _ingredientService;
    private readonly RecipeIngredientService _recipeIngredientService;

    private ObservableCollection<Ingredients> _ingredients;
    public ObservableCollection<Ingredients> Ingredients
    {
        get => _ingredients;
        set
        {
            _ingredients = value;
            OnPropertyChanged();
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
        }
    }
    private string _newIngredientName;
    public string NewIngredientName
    {
        get => _newIngredientName;
        set
        {
            if(SelectedIngredient != null)
            {
                SelectedIngredient.Ingredient = value;
            }
            _newIngredientName = value;
            OnPropertyChanged();
        }
    }
    public ICommand SaveIngredientCommand { get; }
    public ICommand DeleteIngredientCommand { get; }

    public IngredientsViewModel(IngredientService ingredientService, RecipeIngredientService recipeIngredientService)
    {
        Ingredients = new ObservableCollection<Ingredients>();

        _ingredientService = ingredientService;
        _recipeIngredientService = recipeIngredientService;

        SaveIngredientCommand = new RelayCommand(async _ => await SaveIngredient());
        DeleteIngredientCommand = new RelayCommand<Ingredients>(async ingredient => await DeleteIngredient(ingredient));


        _ = LoadIngredientsAsync();
        _recipeIngredientService = recipeIngredientService;
    }
    public async Task LoadIngredientsAsync()
    {
        Ingredients.Clear();
        var ingredients = await _ingredientService.GetAllIngredientsAsync();
        foreach (var ingredient in ingredients.OrderBy(i => i.Ingredient))
        {
            Ingredients.Add(ingredient);
        }
    }
    private async Task SaveIngredient()
    {
        if (string.IsNullOrWhiteSpace(NewIngredientName))
        {
            MessageBox.Show("Enter ingredient name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedIngredient != null) 
        {
            SelectedIngredient.Ingredient = NewIngredientName;
            await _ingredientService.UpdateIngredientAsync(SelectedIngredient);
            MessageBox.Show($"Ingredient '{NewIngredientName}' updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else 
        {
            if (await _ingredientService.IngredientExistsAsync(NewIngredientName))
            {
                MessageBox.Show($"The ingredient '{NewIngredientName}' already exists.",
                                "Duplicate Ingredient", MessageBoxButton.OK, MessageBoxImage.Warning);

                NewIngredientName = string.Empty;
                return;
            }

            var newIngredient = new Ingredients { Ingredient = NewIngredientName };
            await _ingredientService.AddIngredientAsync(newIngredient);
            MessageBox.Show($"Ingredient '{NewIngredientName}' added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        await LoadIngredientsAsync();
        NewIngredientName = string.Empty;
    }

    private async Task DeleteIngredient(Ingredients ingredient)
    {
        if (ingredient == null)
        {
            MessageBox.Show("Choose ingredient to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (await _recipeIngredientService.IsIngredientUsedAsync(ingredient.Id))
        {
            MessageBox.Show($"Ingredient '{ingredient.Ingredient}' is used in one or more recipes and cannot be deleted.",
                            "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete '{ingredient.Ingredient}'?",
                                     "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _ingredientService.DeleteIngredientAsync(ingredient.Id);
            await LoadIngredientsAsync();
        }
    }
}
