using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Recipes.Model;
public class Recipes
{
    public int Id { get; set; }
    public string Recipe { get; set; }
    public string CookingInstructions { get; set; }
    public bool IsFavorite { get; set; }
    
    public int CookingTimeId { get; set; }

    [ForeignKey("CookingTimeId")]
    public CookingTimes? CookingTime { get; set; }
    public ICollection<RecipeRecipeTags> RecipeRecipeTags { get; set; }
    public ICollection<RecipeIngredients> RecipeIngredients { get; set; }


    [NotMapped]
    public string RecipeTags { get; set; }
}
