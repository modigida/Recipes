using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes.Model;
public class RecipeIngredients
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }

    [ForeignKey("RecipeId")]
    public Model.Recipes? Recipes { get; set; }

    [ForeignKey("IngredientId")]
    public Ingredients? Ingredient { get; set; }
    public int UnitId { get; set; }

    [ForeignKey("UnitId")]
    public Units? Unit { get; set; }

    public double Quantity { get; set; }
}
