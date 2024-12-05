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
            _newIngredientName = value;
            OnPropertyChanged();

            
        }
    }

    public ICommand AddIngredientCommand { get; }
    public ICommand UpdateIngredientCommand { get; }
    public ICommand DeleteIngredientCommand { get; }

    public IngredientsViewModel(IngredientService ingredientService)
    {
        Ingredients = new ObservableCollection<Ingredients>();

        _ingredientService = ingredientService;

        AddIngredientCommand = new RelayCommand(async _ => await AddIngredient());
        UpdateIngredientCommand = new RelayCommand(async _ => await UpdateIngredient());
        DeleteIngredientCommand = new RelayCommand(async _ => await DeleteIngredient());

        _ = LoadIngredientsAsync();
    }

    private async Task LoadIngredientsAsync()
    {
        Ingredients.Clear();
        var ingredients = await _ingredientService.GetAllIngredientsAsync();
        foreach (var ingredient in ingredients.OrderBy(i => i.Ingredient))
        {
            Ingredients.Add(ingredient);
        }
    }

    private async Task AddIngredient()
    {
        if (string.IsNullOrWhiteSpace(NewIngredientName))
        {
            MessageBox.Show("Enter ingredient name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var newIngredient = new Ingredients { Ingredient = NewIngredientName };
        await _ingredientService.AddIngredientAsync(newIngredient);
        await LoadIngredientsAsync();
        NewIngredientName = string.Empty;
    }

    private async Task UpdateIngredient()
    {
        if (SelectedIngredient == null)
        {
            MessageBox.Show("Choose ingredient to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(NewIngredientName))
        {
            MessageBox.Show("Enter a new ingredient name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Uppdatera ingrediensens namn
        SelectedIngredient.Ingredient = NewIngredientName;
        await _ingredientService.UpdateIngredientAsync(SelectedIngredient);
        await LoadIngredientsAsync();

        // Rensa NewIngredientName efter uppdatering
        NewIngredientName = string.Empty;
    }

    private async Task DeleteIngredient()
    {
        if (SelectedIngredient == null)
        {
            MessageBox.Show("Choose ingredient to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Sure to delete '{SelectedIngredient.Ingredient}'?",
                                     "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _ingredientService.DeleteIngredientAsync(SelectedIngredient.Id);
            await LoadIngredientsAsync();
        }
    }
}
