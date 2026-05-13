using FINALPROJECT.Models;

namespace FINALPROJECT.Repositories
{
    public interface IVideoRepository
    {
        IEnumerable<Video> GetAll();
        Video? GetById(int id);
        IEnumerable<Video> GetByCategory(string category);
        Video? GetFeatured();
        Video? GetNext(int currentId);
        void Add(Video video);
        void Update(Video video);
        void Delete(int id);
    }
}
