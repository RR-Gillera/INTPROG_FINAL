using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FINALPROJECT.Models;

namespace FINALPROJECT.Data
{
    public class FlavorlyDbContext : IdentityDbContext<User>
    {
        public FlavorlyDbContext(DbContextOptions<FlavorlyDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
        public DbSet<RecipeStep> RecipeSteps => Set<RecipeStep>();
        public DbSet<Video> Videos => Set<Video>();
        public DbSet<Tip> Tips => Set<Tip>();
        public DbSet<Collection> Collections => Set<Collection>();
        public DbSet<CollectionRecipe> CollectionRecipes => Set<CollectionRecipe>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Follow> Follows => Set<Follow>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Recipe → Author (restrict delete to avoid cascade cycles)
            builder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Video → Author
            builder.Entity<Video>()
                .HasOne(v => v.Author)
                .WithMany(u => u.Videos)
                .HasForeignKey(v => v.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tip → Author
            builder.Entity<Tip>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Tips)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Collection → Owner
            builder.Entity<Collection>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Collections)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review → Author
            builder.Entity<Review>()
                .HasOne(r => r.Author)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review → Recipe
            builder.Entity<Review>()
                .HasOne(r => r.Recipe)
                .WithMany(rec => rec.Reviews)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Follow relationships
            builder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // CollectionRecipe
            builder.Entity<CollectionRecipe>()
                .HasOne(cr => cr.Collection)
                .WithMany(c => c.CollectionRecipes)
                .HasForeignKey(cr => cr.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CollectionRecipe>()
                .HasOne(cr => cr.Recipe)
                .WithMany()
                .HasForeignKey(cr => cr.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
