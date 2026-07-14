using Plugin.Firebase.Storage;

namespace Plated.Core.Services;

public class FirebaseStorageService : IStorageService
{
    public async Task<string> UploadPlatePhotoAsync(string localFilePath, string plateId)
    {
        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(localFilePath)}";
        var storageRef = CrossFirebaseStorage.Current
            .GetRootReference()
            .GetChild($"plates/{plateId}/{fileName}");

        var uploadTask = storageRef.PutFile(localFilePath);
        await uploadTask.AwaitAsync();

        return await storageRef.GetDownloadUrlAsync();
    }
}
