namespace Recipes.Model;
public class RecipeRecipeTags
{
    public int RecipeId { get; set; }
    public Recipes Recipe { get; set; }

    public int RecipeTagId { get; set; }
    public RecipeTags RecipeTag { get; set; }
}
