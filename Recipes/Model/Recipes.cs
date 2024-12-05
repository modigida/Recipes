using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes.Model;
public class Recipes
{
    public int Id { get; set; }
    public string Recipe { get; set; }
    public string CookingInstructions { get; set; }
    public bool IsFavorite { get; set; }

    [ForeignKey("CookingTimes")]
    public int CookingTimeId { get; set; }

    public ICollection<RecipeRecipeTags> RecipeRecipeTags { get; set; } // Koppling till taggar
    public ICollection<RecipeIngredients> RecipeIngredients { get; set; }
}
