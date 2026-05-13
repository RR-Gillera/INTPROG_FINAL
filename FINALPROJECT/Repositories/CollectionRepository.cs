using FINALPROJECT.Data;
using FINALPROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Repositories
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly FlavorlyDbContext _db;
        public CollectionRepository(FlavorlyDbContext db) => _db = db;

        public IEnumerable<Collection> GetAll() =>
            _db.Collections
               .Include(c => c.Owner)
               .Include(c => c.CollectionRecipes)
                   .ThenInclude(cr => cr.Recipe)
               .OrderByDescending(c => c.CreatedAt)
               .ToList();

        public Collection? GetById(int id) =>
            _db.Collections
               .Include(c => c.Owner)
               .Include(c => c.CollectionRecipes)
                   .ThenInclude(cr => cr.Recipe)
                       .ThenInclude(r => r!.Author)
               .FirstOrDefault(c => c.Id == id);

        public IEnumerable<Collection> GetByOwner(string ownerId) =>
            _db.Collections
               .Include(c => c.Owner)
               .Include(c => c.CollectionRecipes)
                   .ThenInclude(cr => cr.Recipe)
               .Where(c => c.OwnerId == ownerId)
               .OrderByDescending(c => c.CreatedAt)
               .ToList();

        public void Add(Collection collection)    { _db.Collections.Add(collection);    _db.SaveChanges(); }
        public void Update(Collection collection) { _db.Collections.Update(collection); _db.SaveChanges(); }
        public void Delete(int id)
        {
            var c = _db.Collections.Find(id);
            if (c != null) { _db.Collections.Remove(c); _db.SaveChanges(); }
        }

        public void AddRecipeToCollection(int collectionId, int recipeId)
        {
            if (!_db.CollectionRecipes.Any(cr => cr.CollectionId == collectionId && cr.RecipeId == recipeId))
            {
                _db.CollectionRecipes.Add(new CollectionRecipe
                {
                    CollectionId = collectionId,
                    RecipeId = recipeId,
                    AddedAt = DateTime.UtcNow
                });
                _db.SaveChanges();
            }
        }

        public void RemoveRecipeFromCollection(int collectionId, int recipeId)
        {
            var cr = _db.CollectionRecipes.FirstOrDefault(x => x.CollectionId == collectionId && x.RecipeId == recipeId);
            if (cr != null) { _db.CollectionRecipes.Remove(cr); _db.SaveChanges(); }
        }

        public int Count() => _db.Collections.Count();
    }
}
