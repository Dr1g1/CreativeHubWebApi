using CreativeHubWebApp.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace CreativeHubWebApp.Services
{

    public class GridFsService
    {
        private readonly MongoContext _ctx;
        public GridFsService(MongoContext ctx) => _ctx = ctx;

        // prima stream fajla i ime, vraća id pod kojim je sacuvan
        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            var id = await _ctx.GridFs.UploadFromStreamAsync(fileName, stream);
            return id.ToString();   // ObjectId pretvaramo u string da ga cuvamo u Resource
        }

        // prima id, vraca sadrzaj fajla kao bajtove
        public async Task<byte[]> DownloadAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            return await _ctx.GridFs.DownloadAsBytesAsync(objectId);
        }

        // brisanje fajla, tj kad se obrise resurs
        public async Task DeleteAsync(string fileId)
        {
            var objectId = ObjectId.Parse(fileId);
            await _ctx.GridFs.DeleteAsync(objectId);
        }
    }
}