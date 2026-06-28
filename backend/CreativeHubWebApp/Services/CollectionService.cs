using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;

namespace CreativeHubWebApp.Services
{

    public class CollectionService
    {
        private readonly CollectionRepository _collections;
        private readonly ResourceRepository _resources;

        public CollectionService(CollectionRepository collections, ResourceRepository resources)
        {
            _collections = collections;
            _resources = resources;
        }

        public async Task<List<CollectionResponseDto>> GetByOwnerAsync(string ownerId)
        {
            var list = await _collections.GetByOwnerAsync(ownerId);
            return list.Select(CollectionResponseDto.From).ToList();
        }

        public async Task<CollectionResponseDto?> GetByIdAsync(string id)
        {
            var c = await _collections.GetByIdAsync(id);
            return c is null ? null : CollectionResponseDto.From(c);
        }

        public async Task<CollectionResponseDto> CreateAsync(CreateCollectionDto dto, string ownerId)
        {
            var collection = new ResourceCollection
            {
                Name = dto.Name,
                Description = dto.Description,
                OwnerId = ownerId,
                IsPublic = dto.IsPublic
            };
            await _collections.CreateAsync(collection);
            return CollectionResponseDto.From(collection);
        }

        public async Task AddResourceAsync(string collectionId, string resourceId, string requesterId)
        {
            var collection = await _collections.GetByIdAsync(collectionId);
            if (collection is null)
                throw new InvalidOperationException("Kolekcija ne postoji.");
            if (collection.OwnerId != requesterId)
                throw new UnauthorizedAccessException("Nije tvoja kolekcija.");

            // provera jel resurs uopste postoji
            var resource = await _resources.GetByIdAsync(resourceId);
            if (resource is null)
                throw new InvalidOperationException("Resurs ne postoji.");

            await _collections.AddResourceAsync(collectionId, resourceId);
        }

        public async Task RemoveResourceAsync(string collectionId, string resourceId, string requesterId)
        {
            var collection = await _collections.GetByIdAsync(collectionId);
            if (collection is null)
                throw new InvalidOperationException("Kolekcija ne postoji.");
            if (collection.OwnerId != requesterId)
                throw new UnauthorizedAccessException("Nije tvoja kolekcija.");

            await _collections.RemoveResourceAsync(collectionId, resourceId);
        }

        public async Task<bool> DeleteAsync(string id, string requesterId)
        {
            var collection = await _collections.GetByIdAsync(id);
            if (collection is null) return false;
            if (collection.OwnerId != requesterId)
                throw new UnauthorizedAccessException("Nije tvoja kolekcija.");
            return await _collections.DeleteAsync(id);
        }
    }
}