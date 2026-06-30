using CreativeHubWebApp.DTO;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CreativeHubWebApp.Repositories
{

    public class StatisticsRepository
    {
        private readonly MongoContext _ctx;
        public StatisticsRepository(MongoContext ctx) => _ctx = ctx;

        
        public async Task<BsonDocument?> GetResourceDetailAsync(string resourceId)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("_id", ObjectId.Parse(resourceId))),

                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "OwnerId" },
                    { "foreignField", "_id" },
                    { "as", "creator" }
                }),
                
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$creator" },
                    { "preserveNullAndEmptyArrays", true }
                }),

                
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "reviews" },
                    { "localField", "_id" },
                    { "foreignField", "ResourceId" },
                    { "as", "reviews" }
                })
            };

            return await _ctx.Resources
                .Aggregate<BsonDocument>(pipeline)
                .FirstOrDefaultAsync();
        }

        //najaktivniji kreatori broj resursa i ukupno skidanja se gleda
        public async Task<List<TopCreatorDto>> GetTopCreatorsAsync(int limit = 10)
        {
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$OwnerId" },
                    { "resourceCount", new BsonDocument("$sum", 1) },
                    { "totalDownloads", new BsonDocument("$sum", "$Downloads") }
                }),
                new BsonDocument("$sort", new BsonDocument("totalDownloads", -1)),
                new BsonDocument("$limit", limit),
    
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "users" },
                    { "localField", "_id" },
                    { "foreignField", "_id" },
                    { "as", "user" }
                }),
                new BsonDocument("$unwind", new BsonDocument
                {
                    { "path", "$user" },
                    { "preserveNullAndEmptyArrays", true }
                })
            };

            var docs = await _ctx.Resources.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return docs.Select(d => new TopCreatorDto
            {
                CreatorId = d["_id"].ToString()!,
                Username = d.Contains("user") ? d["user"]["Username"].AsString : "(nepoznat)",
                ResourceCount = d["resourceCount"].AsInt32,
                TotalDownloads = d["totalDownloads"].AsInt32
            }).ToList();
        }

        //histogram xa raspodelu ocena
        public async Task<List<RatingBucketDto>> GetRatingDistributionAsync()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$bucket", new BsonDocument
            {
                { "groupBy", "$AverageRating" },
                { "boundaries", new BsonArray { 0, 1, 2, 3, 4, 5.01 } },  
                { "default", "ostalo" },
                { "output", new BsonDocument
                    {
                        { "count", new BsonDocument("$sum", 1) }
                    }
                }
            })
            };

            var docs = await _ctx.Resources.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return docs.Select(d =>
            {
                var lower = d["_id"].IsBsonNull ? "?" : d["_id"].ToString();
                return new RatingBucketDto
                {
                    Range = $"{lower}+",
                    Count = d["count"].AsInt32
                };
            }).ToList();
        }
    }
}