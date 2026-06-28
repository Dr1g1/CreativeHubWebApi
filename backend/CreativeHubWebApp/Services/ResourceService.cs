using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;

namespace CreativeHubWebApp.Services
{
    // URADI KAD SE VRATIS IZ TERETANE PLS
    public class ResourceService
    {
        private readonly ResourceRepository _resources;
        private readonly GridFsService _gridFs;

        public ResourceService(ResourceRepository resources, GridFsService gridFs)
        {
            _resources = resources;
            _gridFs = gridFs;
        }

        public async Task<List<ResourceResponseDto>> GetAllAsync()
        {
            var list = await _resources.GetAllAsync();
            return list.Select(ResourceResponseDto.From).ToList();
        }

        public async Task<ResourceResponseDto?> GetByIdAsync(string id)
        {
            var r = await _resources.GetByIdAsync(id);
            return r is null ? null : ResourceResponseDto.From(r);
        }

        // kreiranje resursa SA fajlom
        public async Task<ResourceResponseDto> CreateWithFileAsync(
            CreateResourceDto dto, string ownerId, Stream fileStream, string fileName, long fileSize)
        {
            var fileId = await _gridFs.UploadAsync(fileStream, fileName);

            var resource = new Resource
            {
                Title = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                Tags = dto.Tags,
                ContentType = ContentType.File,
                FileId = fileId,
                FileFormat = Path.GetExtension(fileName).TrimStart('.').ToLower(),
                FileSizeBytes = fileSize,
                OwnerId = ownerId
            };

            await _resources.CreateAsync(resource);
            return ResourceResponseDto.From(resource);
        }

        // kreiranje palete 
        public async Task<ResourceResponseDto> CreatePaletteAsync(CreateResourceDto dto, string ownerId)
        {
            var resource = new Resource
            {
                Title = dto.Title,
                Description = dto.Description,
                Type = ResourceType.ColorPalette,
                Tags = dto.Tags,
                ContentType = ContentType.Inline,
                Colors = dto.Colors,
                OwnerId = ownerId
            };

            await _resources.CreateAsync(resource);
            return ResourceResponseDto.From(resource);
        }

        // kad skidamo nesto vracamo bajtove fajla i uvecavamo brojac
        public async Task<(byte[] data, string fileName)?> DownloadAsync(string id)
        {
            var resource = await _resources.GetByIdAsync(id);
            if (resource is null || resource.FileId is null) return null;

            var data = await _gridFs.DownloadAsync(resource.FileId);
            await _resources.IncrementDownloadsAsync(id);   // $inc

            var fileName = $"{resource.Title}.{resource.FileFormat}";
            return (data, fileName);
        }

        public async Task<bool> DeleteAsync(string id, string requesterId)
        {
            var resource = await _resources.GetByIdAsync(id);
            if (resource is null) return false;
            if (resource.OwnerId != requesterId)
                throw new UnauthorizedAccessException("Možeš obrisati samo svoje resurse.");

            if (resource.FileId is not null)
                await _gridFs.DeleteAsync(resource.FileId);   

            return await _resources.DeleteAsync(id);
        }
    }
}