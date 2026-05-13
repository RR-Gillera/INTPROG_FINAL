using FINALPROJECT.Data;
using FINALPROJECT.Models;
using Microsoft.EntityFrameworkCore;

namespace FINALPROJECT.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly FlavorlyDbContext _db;
        public VideoRepository(FlavorlyDbContext db) => _db = db;

        public IEnumerable<Video> GetAll() =>
            _db.Videos
               .Include(v => v.Author)
               .OrderByDescending(v => v.CreatedAt)
               .ToList();

        public Video? GetById(int id) =>
            _db.Videos
               .Include(v => v.Author)
               .FirstOrDefault(v => v.Id == id);

        public IEnumerable<Video> GetByCategory(string category)
        {
            var q = _db.Videos.Include(v => v.Author).AsQueryable();
            if (!string.IsNullOrWhiteSpace(category) && category.ToLower() != "all")
                q = q.Where(v => v.Category.ToLower() == category.ToLower());
            return q.OrderByDescending(v => v.CreatedAt).ToList();
        }

        public Video? GetFeatured() =>
            _db.Videos
               .Include(v => v.Author)
               .Where(v => v.IsFeatured)
               .OrderByDescending(v => v.ViewCount)
               .FirstOrDefault()
            ?? _db.Videos.Include(v => v.Author).OrderByDescending(v => v.ViewCount).FirstOrDefault();

        public Video? GetNext(int currentId) =>
            _db.Videos
               .Include(v => v.Author)
               .Where(v => v.Id != currentId)
               .OrderByDescending(v => v.ViewCount)
               .FirstOrDefault();

        public void Add(Video video)    { _db.Videos.Add(video);    _db.SaveChanges(); }
        public void Update(Video video) { _db.Videos.Update(video); _db.SaveChanges(); }
        public void Delete(int id)
        {
            var v = _db.Videos.Find(id);
            if (v != null) { _db.Videos.Remove(v); _db.SaveChanges(); }
        }
    }
}
