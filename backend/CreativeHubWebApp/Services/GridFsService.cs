using CreativeHubWebApp.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace CreativeHubWebApp.Services
{

    public class GridFsService
    {
        private readonly MongoContext _ctx;
        public GridFsService(MongoContext ctx) => _ctx = ctx;

        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            var id = await _ctx.GridFs.UploadFromStreamAsync(fileName, stream);
            return id.ToString();   
        }

        public async Task<byte[]> DownloadAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            return await _ctx.GridFs.DownloadAsBytesAsync(objectId);
        }

        public async Task DeleteAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            await _ctx.GridFs.DeleteAsync(objectId);
        }

        public async Task<(byte[] data, string fileName)> DownloadWithNameAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            var filter = Builders<GridFSFileInfo>.Filter.Eq(f => f.Id, objectId);
            var info = await (await _ctx.GridFs.FindAsync(filter)).FirstOrDefaultAsync();
            var data = await _ctx.GridFs.DownloadAsBytesAsync(objectId);
            return (data, info?.Filename ?? "file");
        }
    }
}