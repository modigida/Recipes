﻿using Recipes.Commands;
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
            _newIngredientName = value;
            OnPropertyChanged();

            
        }
    }

    public ICommand AddIngredientCommand { get; }
    public ICommand UpdateIngredientCommand { get; }
    public ICommand DeleteIngredientCommand { get; }

    public IngredientsViewModel(IngredientService ingredientService, RecipeIngredientService recipeIngredientService)
    {
        Ingredients = new ObservableCollection<Ingredients>();

        _ingredientService = ingredientService;
        _recipeIngredientService = recipeIngredientService;

        AddIngredientCommand = new RelayCommand(async _ => await AddIngredient());
        UpdateIngredientCommand = new RelayCommand(async _ => await UpdateIngredient());
        DeleteIngredientCommand = new RelayCommand(async _ => await DeleteIngredient());

        _ = LoadIngredientsAsync();
        _recipeIngredientService = recipeIngredientService;
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

        if (await _ingredientService.IngredientExistsAsync(NewIngredientName))
        {
            MessageBox.Show($"The ingredient '{NewIngredientName}' already exists.",
                            "Duplicate Ingredient", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        SelectedIngredient.Ingredient = NewIngredientName;
        await _ingredientService.UpdateIngredientAsync(SelectedIngredient);
        await LoadIngredientsAsync();

        NewIngredientName = string.Empty;
    }

    private async Task DeleteIngredient()
    {
        if (SelectedIngredient == null)
        {
            MessageBox.Show("Choose ingredient to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (await _recipeIngredientService.IsIngredientUsedAsync(SelectedIngredient.Id))
        {
            MessageBox.Show($"Ingredient '{SelectedIngredient.Ingredient}' is used in one or more recipes and cannot be deleted.",
                            "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to delete '{SelectedIngredient.Ingredient}'?",
                                     "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            await _ingredientService.DeleteIngredientAsync(SelectedIngredient.Id);
            await LoadIngredientsAsync();
        }
    }
}
