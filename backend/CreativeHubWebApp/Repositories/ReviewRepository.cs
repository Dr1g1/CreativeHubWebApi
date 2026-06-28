using CreativeHubWebApp.Models;
using MongoDB.Driver;

namespace CreativeHubWebApp.Repositories
{

    public class ReviewRepository
    {
        private readonly MongoContext _ctx;
        public ReviewRepository(MongoContext ctx) => _ctx = ctx;

        public async Task<List<Review>> GetByResourceAsync(string resourceId) =>
            await _ctx.Reviews.Find(r => r.ResourceId == resourceId)
                              .SortByDescending(r => r.CreatedAt)
                              .ToListAsync();
    }
}