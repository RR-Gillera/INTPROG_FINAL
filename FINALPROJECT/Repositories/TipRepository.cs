using FINALPROJECT.Data;
using FINALPROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Repositories
{
    public class TipRepository : ITipRepository
    {
        private readonly FlavorlyDbContext _db;
        public TipRepository(FlavorlyDbContext db) => _db = db;

        public IEnumerable<Tip> GetAll() =>
            _db.Tips
               .Include(t => t.Author)
               .OrderByDescending(t => t.LikeCount)
               .ToList();

        public Tip? GetById(int id) =>
            _db.Tips.Include(t => t.Author).FirstOrDefault(t => t.Id == id);

        public IEnumerable<Tip> GetByAuthor(string authorId) =>
            _db.Tips.Include(t => t.Author).Where(t => t.AuthorId == authorId).ToList();

        public void Add(Tip tip)    { _db.Tips.Add(tip);    _db.SaveChanges(); }
        public void Update(Tip tip) { _db.Tips.Update(tip); _db.SaveChanges(); }
        public void Delete(int id)
        {
            var t = _db.Tips.Find(id);
            if (t != null) { _db.Tips.Remove(t); _db.SaveChanges(); }
        }
    }
}
