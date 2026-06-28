using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;
using MongoDB.Driver;

namespace CreativeHubWebApp.Services
{

    public class ReviewService
    {
        private readonly MongoContext _ctx;
        private readonly ReviewRepository _reviews;

        public ReviewService(MongoContext ctx, ReviewRepository reviews)
        {
            _ctx = ctx;
            _reviews = reviews;
        }

        public Task<List<Review>> GetByResourceAsync(string resourceId) =>
            _reviews.GetByResourceAsync(resourceId);

        public async Task<ReviewResponseDto> AddReviewAsync(
            string resourceId, string userId, CreateReviewDto dto)
        {
            using var session = await _ctx.Client.StartSessionAsync();

            var review = new Review
            {
                ResourceId = resourceId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await session.WithTransactionAsync(async (s, ct) =>
            {
                await _ctx.Reviews.InsertOneAsync(s, review, cancellationToken: ct);
                await RecalculateRatingAsync(s, resourceId, ct);
                return true;
            });

            return ReviewResponseDto.From(review);
        }

        // samo onaj ko je napisao recenziju sme da je menja
        public async Task<ReviewResponseDto> UpdateReviewAsync(
            string resourceId, string userId, CreateReviewDto dto)
        {
            using var session = await _ctx.Client.StartSessionAsync();
            Review? updated = null;

            await session.WithTransactionAsync(async (s, ct) =>
            {
                // trazimo recenziju korisnika
                var existing = await _ctx.Reviews
                    .Find(s, r => r.ResourceId == resourceId && r.UserId == userId)
                    .FirstOrDefaultAsync(ct);

                if (existing is null)
                    throw new InvalidOperationException("Nemaš recenziju za ovaj resurs.");

                // azuriramo ocenu i komentar
                var update = Builders<Review>.Update
                    .Set(r => r.Rating, dto.Rating)
                    .Set(r => r.Comment, dto.Comment);

                await _ctx.Reviews.UpdateOneAsync(s, r => r.Id == existing.Id, update, cancellationToken: ct);

                await RecalculateRatingAsync(s, resourceId, ct);

                existing.Rating = dto.Rating;
                existing.Comment = dto.Comment;
                updated = existing;
                return true;
            });

            return ReviewResponseDto.From(updated!);
        }

        // samo onaj ko je napisao sme da je obrise
        public async Task<bool> DeleteReviewAsync(string resourceId, string userId)
        {
            using var session = await _ctx.Client.StartSessionAsync();
            var deleted = false;

            await session.WithTransactionAsync(async (s, ct) =>
            {
                var result = await _ctx.Reviews.DeleteOneAsync(
                    s, r => r.ResourceId == resourceId && r.UserId == userId, cancellationToken: ct);

                if (result.DeletedCount == 0)
                    throw new InvalidOperationException("Nemaš recenziju za ovaj resurs.");

                await RecalculateRatingAsync(s, resourceId, ct);
                deleted = true;
                return true;
            });

            return deleted;
        }

        // pomocna fja koja racuna ponovo avg rejting i broj ocena
        private async Task RecalculateRatingAsync(
            IClientSessionHandle s, string resourceId, CancellationToken ct)
        {
            var allReviews = await _ctx.Reviews
                .Find(s, r => r.ResourceId == resourceId)
                .ToListAsync(ct);

            // ako nema recenzija ocena 0
            var avg = allReviews.Count > 0 ? allReviews.Average(r => r.Rating) : 0;
            var count = allReviews.Count;

            var update = Builders<Resource>.Update
                .Set(r => r.AverageRating, Math.Round(avg, 2))
                .Set(r => r.ReviewCount, count);

            await _ctx.Resources.UpdateOneAsync(s, r => r.Id == resourceId, update, cancellationToken: ct);
        }
    }
}