using Recipes.Model;
using Recipes.Services;
using System.Collections.ObjectModel;

namespace Recipes.ViewModel;
public class DetailedRecipeLoadDataViewModel : BaseViewModel
{
    private readonly DetailedRecipeViewModel _detailedRecipeViewModel;
    public DetailedRecipeLoadDataViewModel(DetailedRecipeViewModel detailedRecipeViewModel)
    {
        _detailedRecipeViewModel = detailedRecipeViewModel;
    }
    public async void LoadAllIngredients(IngredientService ingredientService)
    {
        var ingredients = await ingredientService.GetAllIngredientsAsync();
        _detailedRecipeViewModel.AllIngredients = new ObservableCollection<Ingredients>(ingredients);
    }
    public async void LoadAllLists(GetStaticListDataService getStaticListDataService)
    {
        DetailedRecipeViewModel.Units = new ObservableCollection<Units>(getStaticListDataService.GetUnits());
        DetailedRecipeViewModel.CookingTimes = new ObservableCollection<CookingTimes>(getStaticListDataService.GetCookingTimes());
        _detailedRecipeViewModel.RecipeTags = new ObservableCollection<RecipeTags>(getStaticListDataService.GetRecipeTags());
    }

    public async Task<List<RecipeTags>> LoadTags(List<RecipeTags> recipeTags, TagService tagService, Model.Recipes recipe = null)
    {
        if (recipe?.Id > 0)
        {
            var tagsForRecipe = await tagService.GetTagsForRecipeAsync(recipe.Id);

            foreach (var tag in recipeTags)
            {
                tag.IsSelected = tagsForRecipe.Contains(tag.Id);
            }
        }
        else
        {
            foreach (var tag in recipeTags)
            {
                tag.IsSelected = false;
            }
        }
        return recipeTags;
    }
    public async Task<Model.Recipes> LoadRecipe(int recipeId, RecipeService recipeService)
    {
        if (recipeId > 0)
        {
            return await recipeService.GetRecipeByIdAsync(recipeId);
        }
        else
        {
            return new Model.Recipes
            {
                Recipe = "New Recipe",
                CookingInstructions = string.Empty,
                CookingTimeId = DetailedRecipeViewModel.CookingTimes.FirstOrDefault(ct => ct.Id == 2)?.Id ?? 0,
                RecipeIngredients = new ObservableCollection<RecipeIngredients>()
            };
        }
    }
}
