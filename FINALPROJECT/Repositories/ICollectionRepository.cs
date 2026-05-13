using FINALPROJECT.Models;

namespace FINALPROJECT.Repositories
{
    public interface ICollectionRepository
    {
        IEnumerable<Collection> GetAll();
        Collection? GetById(int id);
        IEnumerable<Collection> GetByOwner(string ownerId);
        void Add(Collection collection);
        void Update(Collection collection);
        void Delete(int id);
        void AddRecipeToCollection(int collectionId, int recipeId);
        void RemoveRecipeFromCollection(int collectionId, int recipeId);
        int Count();
    }
}
