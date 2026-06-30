using CreativeHubWebApp.Models;
using MongoDB.Driver;

namespace CreativeHubWebApp.Repositories
{

    public class ResourceRepository
    {
        private readonly MongoContext _ctx;
        public ResourceRepository(MongoContext ctx) => _ctx = ctx;

        public async Task<List<Resource>> GetAllAsync() =>
            await _ctx.Resources.Find(_ => true)
                                .SortByDescending(r => r.CreatedAt)
                                .ToListAsync();

        public async Task<Resource?> GetByIdAsync(string id) =>
            await _ctx.Resources.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Resource resource) =>
            await _ctx.Resources.InsertOneAsync(resource);

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _ctx.Resources.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }

        //$inc operator poveca se kao atomska operaacija
        public async Task IncrementDownloadsAsync(string id)
        {
            var update = Builders<Resource>.Update.Inc(r => r.Downloads, 1);
            await _ctx.Resources.UpdateOneAsync(r => r.Id == id, update);
        }

        public async Task<List<Resource>> GetByOwnerAsync(string ownerId) =>
            await _ctx.Resources.Find(r => r.OwnerId == ownerId)
                        .SortByDescending(r => r.CreatedAt)
                        .ToListAsync();

        
        public async Task<List<Resource>> GetByOwnerInSessionAsync(IClientSessionHandle session, string ownerId) =>
            await _ctx.Resources.Find(session, r => r.OwnerId == ownerId).ToListAsync();

        public async Task DeleteByOwnerAsync(IClientSessionHandle session, string ownerId) =>
            await _ctx.Resources.DeleteManyAsync(session, r => r.OwnerId == ownerId);
    }
}