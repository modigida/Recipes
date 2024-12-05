using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Recipes.Model;

namespace Recipes.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<CookingTimes> CookingTimes { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<Model.Recipes> Recipes { get; set; }
        public DbSet<RecipeTags> RecipeTags { get; set; }
        public DbSet<RecipeRecipeTags> RecipeRecipeTags { get; set; }
        public DbSet<Units> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = "(localdb)\\MSSQLLocalDB",
                InitialCatalog = "Recipes",
                TrustServerCertificate = true,
                IntegratedSecurity = true
            }.ToString();

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RecipeIngredients>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            
            modelBuilder.Entity<RecipeIngredients>()
                .HasOne(ri => ri.Recipes)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredients>()
                .HasOne(ri => ri.Ingredient)
                .WithMany()
                .HasForeignKey(ri => ri.IngredientId);

            modelBuilder.Entity<RecipeIngredients>()
                .HasOne(ri => ri.Unit)
                .WithMany()
                .HasForeignKey(ri => ri.UnitId);

            modelBuilder.Entity<RecipeRecipeTags>()
                .HasKey(rrt => new { rrt.RecipeId, rrt.RecipeTagId });

            modelBuilder.Entity<RecipeRecipeTags>()
                .HasOne(rrt => rrt.Recipe)
                .WithMany(r => r.RecipeRecipeTags)
                .HasForeignKey(rrt => rrt.RecipeId);

            modelBuilder.Entity<RecipeRecipeTags>()
                .HasOne(rrt => rrt.RecipeTag)
                .WithMany(rt => rt.RecipeRecipeTags)
                .HasForeignKey(rrt => rrt.RecipeTagId);
        }
    }
}
