using System.Diagnostics;
using Plugin.Firebase.Storage;

namespace Plated.Core.Services;

public class FirebaseStorageService : IStorageService
{
    private static readonly TimeSpan UploadTimeout = TimeSpan.FromSeconds(30);

    public async Task<string?> TryUploadPlatePhotoAsync(string localFilePath, string plateId)
    {
        try
        {
            var upload = UploadAsync(localFilePath, plateId);
            var finished = await Task.WhenAny(upload, Task.Delay(UploadTimeout));
            if (finished == upload)
            {
                return await upload;
            }

            Debug.WriteLine("[Plated] Photo upload timed out; posting without the photo. " +
                            "Check that Firebase Storage is set up.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Plated] Photo upload failed; posting without the photo: {ex}");
        }

        return null;
    }

    private static async Task<string> UploadAsync(string localFilePath, string plateId)
    {
        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(localFilePath)}";
        var storageRef = CrossFirebaseStorage.Current
            .GetRootReference()
            .GetChild($"plates/{plateId}/{fileName}");

        await storageRef.PutFile(localFilePath).AwaitAsync();
        return await storageRef.GetDownloadUrlAsync();
    }
}
