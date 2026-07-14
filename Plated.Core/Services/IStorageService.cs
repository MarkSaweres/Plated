namespace Plated.Core.Services;

public interface IStorageService
{
    Task<string> UploadPlatePhotoAsync(string localFilePath, string plateId);
}
