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
        return _context.Units.ToList();
    }
    public List<CookingTimes> GetCookingTimes()
    {
        return _context.CookingTimes.ToList();
    }
    public List<RecipeTags> GetRecipeTags()
    {
        return _context.RecipeTags.ToList();
    }
}
