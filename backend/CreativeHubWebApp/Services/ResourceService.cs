using CreativeHubWebApp.DTO;
using CreativeHubWebApp.Models;
using CreativeHubWebApp.Repositories;

namespace CreativeHubWebApp.Services
{
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

        public Task<(byte[] data, string fileName)> GetPreviewAsync(string fileId) =>
            _gridFs.DownloadWithNameAsync(fileId);

        public async Task<ResourceResponseDto?> GetByIdAsync(string id)
        {
            var r = await _resources.GetByIdAsync(id);
            return r is null ? null : ResourceResponseDto.From(r);
        }

        public async Task<ResourceResponseDto> CreateWithFileAsync(
            CreateResourceDto dto, string ownerId, Stream fileStream, string fileName, long fileSize,
            List<(Stream stream, string fileName)> previews)
        {
            var fileId = await _gridFs.UploadAsync(fileStream, fileName);

            var previewIds = new List<string>();
            foreach (var p in previews)
            {
                var pid = await _gridFs.UploadAsync(p.stream, p.fileName);
                previewIds.Add(pid);
            }

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
                OwnerId = ownerId,
                PreviewImageIds = previewIds
            };

            await _resources.CreateAsync(resource);
            return ResourceResponseDto.From(resource);
        }

        public async Task<ResourceResponseDto> CreatePaletteAsync(
            CreateResourceDto dto, string ownerId, List<(Stream stream, string fileName)> previews)
        {
            var previewIds = new List<string>();
            foreach (var p in previews)
            {
                var pid = await _gridFs.UploadAsync(p.stream, p.fileName);
                previewIds.Add(pid);
            }

            var resource = new Resource
            {
                Title = dto.Title,
                Description = dto.Description,
                Type = ResourceType.ColorPalette,
                Tags = dto.Tags,
                ContentType = ContentType.Inline,
                Colors = dto.Colors,
                OwnerId = ownerId,
                PreviewImageIds = previewIds
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
            await _resources.IncrementDownloadsAsync(id);  

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
            foreach (var pid in resource.PreviewImageIds)
                await _gridFs.DeleteAsync(pid);

            return await _resources.DeleteAsync(id);
        }

        public async Task<List<ResourceResponseDto>> GetByOwnerAsync(string ownerId)
        {
            var list = await _resources.GetByOwnerAsync(ownerId);
            return list.Select(ResourceResponseDto.From).ToList();
        }

        public async Task<bool> UpdateAsync(string id, string userId, UpdateResourceDto dto)
        {
            var resource = await _resources.GetByIdAsync(id);
            if (resource is null || resource.OwnerId != userId) return false;

            await _resources.UpdateAsync(id, dto.Title, dto.Description, dto.Tags, dto.Colors);
            return true;
        }

        public async Task<string?> AddPreviewAsync(string id, string userId, Stream stream, string fileName)
        {
            var resource = await _resources.GetByIdAsync(id);
            if (resource is null || resource.OwnerId != userId) return null;

            var fileId = await _gridFs.UploadAsync(stream, fileName);
            await _resources.AddPreviewAsync(id, fileId);
            return fileId;
        }

        public async Task<bool> RemovePreviewAsync(string id, string userId, string fileId)
        {
            var resource = await _resources.GetByIdAsync(id);
            if (resource is null || resource.OwnerId != userId) return false;

            await _resources.RemovePreviewAsync(id, fileId);   
            await _gridFs.DeleteAsync(fileId);                  
            return true;
        }
    }
}