using CreativeHubWebApp.Models;
using MongoDB.Driver;

namespace CreativeHubWebApp.Repositories
{
    public class UserRepository
    {
        private readonly MongoContext _ctx;
        public UserRepository(MongoContext ctx) => _ctx = ctx;


        public async Task<User?> GetByEmailAsync(string email) =>
        await _ctx.Users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _ctx.Users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task<User?> GetByIdAsync(string id) =>
            await _ctx.Users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) =>
            await _ctx.Users.InsertOneAsync(user);

        public async Task UpdateProfileAsync(string id, string displayName, string bio)
        {
            var update = Builders<User>.Update
                .Set(u => u.DisplayName, displayName)
                .Set(u => u.Bio, bio);
            await _ctx.Users.UpdateOneAsync(u => u.Id == id, update);
        }

        public async Task DeleteAsync(IClientSessionHandle session, string id)
        {
            await _ctx.Users.DeleteOneAsync(session, u => u.Id == id);
        }
    }
}
