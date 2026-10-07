namespace Plated.Core.Services;

public interface IStorageService
{
    /// <summary>
    /// Uploads a photo and returns its download URL, or null if the upload failed or timed out
    /// (for example when Firebase Storage isn't set up). Callers should post without the photo.
    /// </summary>
    Task<string?> TryUploadPlatePhotoAsync(string localFilePath, string plateId);
}
