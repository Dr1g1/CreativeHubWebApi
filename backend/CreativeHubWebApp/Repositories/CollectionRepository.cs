using CreativeHubWebApp.Models;
using MongoDB.Driver;

namespace CreativeHubWebApp.Repositories
{

    public class CollectionRepository
    {
        private readonly MongoContext _ctx;
        public CollectionRepository(MongoContext ctx) => _ctx = ctx;

        public async Task<List<ResourceCollection>> GetByOwnerAsync(string ownerId) =>
            await _ctx.Collections.Find(c => c.OwnerId == ownerId)
                                  .SortByDescending(c => c.CreatedAt)
                                  .ToListAsync();

        public async Task<ResourceCollection?> GetByIdAsync(string id) =>
            await _ctx.Collections.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(ResourceCollection collection) =>
            await _ctx.Collections.InsertOneAsync(collection);

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _ctx.Collections.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }

        // $addToSet — dodaj resurs u niz, bez duplikata
        public async Task AddResourceAsync(string collectionId, string resourceId)
        {
            var update = Builders<ResourceCollection>.Update
                .AddToSet(c => c.ResourceIds, resourceId);
            await _ctx.Collections.UpdateOneAsync(c => c.Id == collectionId, update);
        }

        // $pull — izbaci resurs iz niza
        public async Task RemoveResourceAsync(string collectionId, string resourceId)
        {
            var update = Builders<ResourceCollection>.Update
                .Pull(c => c.ResourceIds, resourceId);
            await _ctx.Collections.UpdateOneAsync(c => c.Id == collectionId, update);
        }
    }
}