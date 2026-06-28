using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Repositories;
using MongoDB.Bson;

namespace CreativeHubWebApp.Services
{

    public class StatisticsService
    {
        private readonly StatisticsRepository _stats;
        public StatisticsService(StatisticsRepository stats) => _stats = stats;

        public Task<List<TopCreatorDto>> GetTopCreatorsAsync(int limit) =>
            _stats.GetTopCreatorsAsync(limit);

        public Task<List<RatingBucketDto>> GetRatingDistributionAsync() =>
            _stats.GetRatingDistributionAsync();

        public async Task<ResourceDetailDto?> GetResourceDetailAsync(string resourceId)
        {
            var doc = await _stats.GetResourceDetailAsync(resourceId);
            if (doc is null) return null;

            var detail = new ResourceDetailDto
            {
                Id = doc["_id"].ToString()!,
                Title = doc.GetValue("Title", "").AsString,
                Description = doc.GetValue("Description", "").AsString,
                Type = doc.GetValue("Type", "").AsString,
                Tags = doc.Contains("Tags")
                    ? doc["Tags"].AsBsonArray.Select(t => t.AsString).ToList()
                    : new(),
                Downloads = doc.GetValue("Downloads", 0).ToInt32(),
                AverageRating = doc.GetValue("AverageRating", 0.0).ToDouble(),
                ReviewCount = doc.GetValue("ReviewCount", 0).ToInt32()
            };

            if (doc.Contains("creator") && !doc["creator"].IsBsonNull)
            {
                var c = doc["creator"].AsBsonDocument;
                detail.Creator = new CreatorDto
                {
                    Id = c["_id"].ToString()!,
                    Username = c.GetValue("Username", "").AsString,
                    DisplayName = c.GetValue("DisplayName", "").AsString
                };
            }

            if (doc.Contains("reviews"))
            {
                detail.Reviews = doc["reviews"].AsBsonArray.Select(r =>
                {
                    var rd = r.AsBsonDocument;
                    return new ReviewWithUserDto
                    {
                        Id = rd["_id"].ToString()!,
                        Rating = rd.GetValue("Rating", 0).ToInt32(),
                        Comment = rd.GetValue("Comment", "").AsString,
                        CreatedAt = rd.GetValue("CreatedAt", DateTime.MinValue).ToUniversalTime()
                    };
                }).ToList();
            }

            return detail;
        }
    }
}