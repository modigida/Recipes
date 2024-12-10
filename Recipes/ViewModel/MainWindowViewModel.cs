using Recipes.Commands;
using System.Windows.Input;

namespace Recipes.ViewModel;
public class MainWindowViewModel : BaseViewModel
{
    private bool _isRecipeViewVisible;
    public bool IsRecipeViewVisible
    {
        get => _isRecipeViewVisible;
        set
        {
            _isRecipeViewVisible = value;
            OnPropertyChanged();
        }
    }

    private bool _isDetailedViewVisible;
    public bool IsDetailedViewVisible
    {
        get => _isDetailedViewVisible;
        set
        {
            _isDetailedViewVisible = value;
            OnPropertyChanged();

            if (_isDetailedViewVisible)
            {
                if (RecipeVM.SelectedRecipe != null)
                {
                    DetailedVM.LoadData(RecipeVM.SelectedRecipe);
                }
                else
                {
                    DetailedVM.LoadData();
                }
            }
        }
    }
    private bool _isIngredientViewVisible;
    public bool IsIngredientViewVisible
    {
        get => _isIngredientViewVisible;
        set
        {
            _isIngredientViewVisible = value;
            OnPropertyChanged();
        }
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
    public void OpenDetailedRecipe()
    {
        IsRecipeViewVisible = false;
        IsIngredientViewVisible = false;
        IsDetailedViewVisible = true;
    }
}
