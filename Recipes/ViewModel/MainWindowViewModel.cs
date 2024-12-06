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
        }
    }

    public ICommand OpenDetailedViewCommand { get; }
    public ICommand ShowRecipeViewCommand { get; }

    public RecipeViewModel RecipeVM { get; }
    public DetailedRecipeViewModel DetailedVM { get; }
    public IngredientsViewModel IngredientsVM { get; }

    public MainWindowViewModel()
    {
        RecipeVM = new(new Services.RecipeService(new Database.AppDbContext()));
        DetailedVM = new(new Services.GetStaticListDataService(new Database.AppDbContext()), 
            new Services.IngredientService(new Database.AppDbContext()), new Services.TagService(new Database.AppDbContext()), 
            new Services.RecipeService(new Database.AppDbContext()), new Services.RecipeIngredientService(new Database.AppDbContext()));
        IngredientsVM = new(new Services.IngredientService(new Database.AppDbContext()), new Services.RecipeIngredientService(new Database.AppDbContext()));

        IsRecipeViewVisible = true;
        IsDetailedViewVisible = false;

        OpenDetailedViewCommand = new RelayCommand(OpenDetailedView);
        ShowRecipeViewCommand = new RelayCommand(ShowRecipeView);
    }

    private void ShowRecipeView(object obj)
    {
        //IsDetailedViewVisible = false;
        //IsRecipeViewVisible = true;
    }

    private void OpenDetailedView(object obj)
    {
        //IsRecipeViewVisible = false;
        //IsDetailedViewVisible = true;
    }
}
