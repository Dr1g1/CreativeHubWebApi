using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CreativeHubWebApp.Repositories
{

    public class SearchRepository
    {
        private readonly MongoContext _ctx;
        public SearchRepository(MongoContext ctx) => _ctx = ctx;

        public async Task<PagedResultDto<Resource>> SearchAsync(ResourceSearchQueryDto q)
        {
            var builder = Builders<Resource>.Filter;
            var filters = new List<FilterDefinition<Resource>>();

            if (!string.IsNullOrWhiteSpace(q.Text))
                filters.Add(builder.Text(q.Text));

            if (q.Type.HasValue)
                filters.Add(builder.Eq(r => r.Type, q.Type.Value));

            if (q.Tags is not null && q.Tags.Count > 0)
                filters.Add(builder.All(r => r.Tags, q.Tags));

            if (!string.IsNullOrWhiteSpace(q.ExcludeOwnerId))
                filters.Add(builder.Ne(r => r.OwnerId, q.ExcludeOwnerId));

            var filter = filters.Count > 0
                ? builder.And(filters)
                : builder.Empty;

            var sort = q.SortBy switch
            {
                "downloads" => Builders<Resource>.Sort.Descending(r => r.Downloads),
                "rating" => Builders<Resource>.Sort.Descending(r => r.AverageRating),
                _ => Builders<Resource>.Sort.Descending(r => r.CreatedAt)
            };

            var total = await _ctx.Resources.CountDocumentsAsync(filter);

            var items = await _ctx.Resources
                .Find(filter)
                .Sort(sort)
                .Skip((q.Page - 1) * q.PageSize)   
                .Limit(q.PageSize)                 
                .ToListAsync();

            return new PagedResultDto<Resource>
            {
                Items = items,
                TotalCount = total,
                Page = q.Page,
                PageSize = q.PageSize
            };
        }

        public async Task<FacetResultDto> GetFacetsAsync()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$facet", new BsonDocument
            {
                { "byType", new BsonArray
                    {
                        new BsonDocument("$group", new BsonDocument
                        {
                            { "_id", "$Type" },
                            { "count", new BsonDocument("$sum", 1) }
                        }),
                        new BsonDocument("$sort", new BsonDocument("count", -1))
                    }
                },
                { "topTags", new BsonArray
                    {
                        new BsonDocument("$unwind", "$Tags"),
                        new BsonDocument("$group", new BsonDocument
                        {
                            { "_id", "$Tags" },
                            { "count", new BsonDocument("$sum", 1) }
                        }),
                        new BsonDocument("$sort", new BsonDocument("count", -1)),
                        new BsonDocument("$limit", 10)
                    }
                }
            })
            };

            var result = await _ctx.Resources
                .Aggregate<BsonDocument>(pipeline)
                .FirstAsync();

            return new FacetResultDto
            {
                ByType = ParseFacet(result["byType"].AsBsonArray),
                TopTags = ParseFacet(result["topTags"].AsBsonArray)
            };
        }

        private static List<FacetCount> ParseFacet(BsonArray array) =>
            array.Select(item =>
            {
                var doc = item.AsBsonDocument;
                return new FacetCount
                {
                    Value = doc["_id"].IsBsonNull ? "(none)" : doc["_id"].ToString()!,
                    Count = doc["count"].AsInt32
                };
            }).ToList();
    }
}