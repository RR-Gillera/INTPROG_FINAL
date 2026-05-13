using FINALPROJECT.Models;

namespace FINALPROJECT.Repositories
{
    public interface ITipRepository
    {
        IEnumerable<Tip> GetAll();
        Tip? GetById(int id);
        IEnumerable<Tip> GetByAuthor(string authorId);
        void Add(Tip tip);
        void Update(Tip tip);
        void Delete(int id);
    }
}
