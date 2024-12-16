using Recipes.Database;
using Recipes.Model;

namespace Recipes.Services;
public class GetStaticListDataService
{
    private readonly AppDbContext _context;
    public GetStaticListDataService(AppDbContext context)
    {
        _context = context;
    }
    public List<Units> GetUnits()
    {
        try
        {
            return _context.Units.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching units: {ex.Message}");
            return new List<Units>();
        }
    }
    public List<CookingTimes> GetCookingTimes()
    {
        try
        {
            return _context.CookingTimes.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching cooking times: {ex.Message}");
            return new List<CookingTimes>();
        }
    }
    public List<RecipeTags> GetRecipeTags()
    {
        try
        {
            return _context.RecipeTags.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching recipe tags: {ex.Message}");
            return new List<RecipeTags>();
        }
    }
}
