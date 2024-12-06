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

    public ObservableCollection<Ingredients> AllIngredients { get; set; }
    public ObservableCollection<Ingredients> FilteredIngredients { get; set; }
    public ObservableCollection<Units> Units { get; set; }
    public ObservableCollection<CookingTimes> CookingTimes { get; set; }
    public ObservableCollection<RecipeTags> RecipeTags { get; set; }
    public ObservableCollection<RecipeIngredients> NewRecipeIngredients { get; set; }


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
    public DetailedRecipeViewModel(GetStaticListDataService staticDataService, 
        IngredientService ingredientService, TagService tagsService, RecipeService recipeService, 
        RecipeIngredientService recipeIngredientService)
    {
        _staticDataService = staticDataService;
        _ingredientService = ingredientService;
        _tagService = tagsService;
        _recipeService = recipeService;
        _recipeIngredientService = recipeIngredientService;

        AllIngredients = new ObservableCollection<Ingredients>();
        FilteredIngredients = new ObservableCollection<Ingredients>();
        NewRecipeIngredients = new ObservableCollection<RecipeIngredients>();


        LoadAllIngredients();

        LoadData();

        if (Recipe == null)
        {
            Recipe = new Model.Recipes
            {
                Recipe = "New Recipe",
                CookingInstructions = string.Empty,
                CookingTimeId = CookingTimes.FirstOrDefault()?.Id ?? 0,
                RecipeIngredients = new List<RecipeIngredients>()
           
             
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
    private async void LoadData()
    {
        Units = new ObservableCollection<Units>(_staticDataService.GetUnits());
        CookingTimes = new ObservableCollection<CookingTimes>(_staticDataService.GetCookingTimes());
        RecipeTags = new ObservableCollection<RecipeTags>(_staticDataService.GetRecipeTags());

        if (Recipe?.Id > 0) // Endast om receptet redan finns
        {
            var tagsForRecipe = await _tagService.GetTagsForRecipeAsync(Recipe.Id);

            // Förutsätter att alla taggar redan finns i den statiska listan
            foreach (var tag in RecipeTags)
            {
                tag.IsSelected = tagsForRecipe.Any(t => t.Id == tag.Id); // Sätt IsSelected baserat på kopplingen
            }

            // Trigga om det behövs
            OnPropertyChanged(nameof(RecipeTags));
        }
        else
        {
            // Om det inte finns ett recept, sätt alla tags till IsSelected = false som standard
            foreach (var tag in RecipeTags)
            {
                tag.IsSelected = false;
            }

            // Trigga om det behövs
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

        // Lägg till den nya ingrediensen i båda kollektionerna
        Recipe.RecipeIngredients.Add(newIngredient);

        if (!NewRecipeIngredients.Any(ri => ri.Ingredient.Ingredient == newIngredient.Ingredient.Ingredient))
        {
            NewRecipeIngredients.Add(newIngredient);
        }


        // Återställ fälten
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
            await _recipeService.UpdateRecipeAsync(Recipe);
        }

        var selectedTagIds = RecipeTags
            .Where(tag => (bool)tag.IsSelected)
            .Select(tag => tag.Id)
            .ToList();
        await _tagService.SaveSelectedTagsAsync(Recipe.Id, selectedTagIds);

        foreach (var recipeIngredient in NewRecipeIngredients)
        {
            var existingIngredient = AllIngredients.FirstOrDefault(i => i.Ingredient == recipeIngredient.Ingredient.Ingredient);

            if (existingIngredient == null)
            {
                recipeIngredient.Ingredient.Id = await _ingredientService.AddIngredientAsync(recipeIngredient.Ingredient);
            }
            else
            {
                recipeIngredient.Ingredient.Id = existingIngredient.Id;
            }

            await _recipeIngredientService.AddRecipeIngredientAsync(Recipe.Id, recipeIngredient.Ingredient.Id, recipeIngredient.Quantity, recipeIngredient.Unit.Id);
        }

        // Rensa nya ingredienser efter sparning
        NewRecipeIngredients.Clear();

        MessageBox.Show("Recipe saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    private async void DeleteRecipe(object obj)
    {
        // DELETE from database
        var result = MessageBox.Show("Are you sure you want to delete this recipe?", "Confirm Delete",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            await _recipeService.DeleteRecipeAsync(Recipe.Id);
            MessageBox.Show("Recipe deleted successfully.", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
    

}
