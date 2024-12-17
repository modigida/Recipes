using Recipes.Commands;
using Recipes.Model;
using System.Windows.Input;

namespace Recipes.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    private bool _isRecipeViewVisible;
    public bool IsRecipeViewVisible
    {
        get => _isRecipeViewVisible;
        set => SetProperty(ref _isRecipeViewVisible, value);
    }

    private bool _isDetailedViewVisible;
    public bool IsDetailedViewVisible
    {
        get => _isDetailedViewVisible;
        set => SetProperty(ref _isDetailedViewVisible, value);
    }

    private bool _isIngredientViewVisible;
    public bool IsIngredientViewVisible
    {
        get => _isIngredientViewVisible;
        set => SetProperty(ref _isIngredientViewVisible, value);
    }

    public ICommand OpenDetailedViewCommand { get; }
    public ICommand ShowRecipeViewCommand { get; }
    public ICommand OpenIngredientsViewCommand { get; }

    public RecipeViewModel RecipeVM { get; }
    public DetailedRecipeViewModel DetailedVM { get; }
    public IngredientsViewModel IngredientsVM { get; }

    public MainWindowViewModel()
    {
        RecipeVM = new(new Services.RecipeService(new Database.AppDbContext()), this);

        DetailedVM = new(new Services.GetStaticListDataService(new Database.AppDbContext()), 
            new Services.IngredientService(new Database.AppDbContext()), new Services.TagService(new Database.AppDbContext()), 
            new Services.RecipeService(new Database.AppDbContext()), 
            new Services.RecipeIngredientService(new Database.AppDbContext()),
            new RecipeViewModel(new Services.RecipeService(new Database.AppDbContext()), this), this);
        
        IngredientsVM = new(new Services.IngredientService(new Database.AppDbContext()), new Services.RecipeIngredientService(new Database.AppDbContext()));

        IsRecipeViewVisible = true;
        IsDetailedViewVisible = false;
        IsIngredientViewVisible = false;

        OpenDetailedViewCommand = new RelayCommand(OpenDetailedView);
        ShowRecipeViewCommand = new RelayCommand(ShowRecipeView);
        OpenIngredientsViewCommand = new RelayCommand(OpenIngredientsView);
    }
    public void ShowRecipeView(object obj)
    {
        RecipeVM.LoadRecipes();
        if (RecipeVM.SortOptions != null)
        {
            RecipeVM.SelectedSortOption = RecipeVM.SortOptions.FirstOrDefault(s => s == "Sort by..");
        }
        IsDetailedViewVisible = false;
        IsIngredientViewVisible = false;
        IsRecipeViewVisible = true;
    }
    public void OpenDetailedView(object obj)
    {
        RecipeVM.SelectedRecipe = null;
        DetailedVM.LoadData();
        IsRecipeViewVisible = false;
        IsIngredientViewVisible = false;
        IsDetailedViewVisible = true;
    }
    private void OpenIngredientsView(object obj)
    {
        IsRecipeViewVisible = false;
        IsDetailedViewVisible = false;
        IsIngredientViewVisible = true; 
    }
    public async void OpenDetailedRecipe()
    {
        if (RecipeVM.SelectedRecipe != null)
        {
            await DetailedVM.LoadData(RecipeVM.SelectedRecipe);
        }
        else
        {
            await DetailedVM.LoadData();
        }

        IsRecipeViewVisible = false;
        IsIngredientViewVisible = false;
        IsDetailedViewVisible = true;
    }
}
