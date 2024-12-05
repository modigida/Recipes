namespace Recipes.Model;
public class RecipeTags
{
    public int Id { get; set; }
    public string Tag { get; set; }

    public ICollection<RecipeRecipeTags> RecipeRecipeTags { get; set; }
}
