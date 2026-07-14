using Microsoft.Maui.Media;

namespace Plated.Core.Services;

public class MediaPickerPhotoCaptureService : IPhotoCaptureService
{
    public bool IsCaptureSupported => MediaPicker.Default.IsCaptureSupported;

    public async Task<string?> CapturePhotoAsync()
    {
        var result = await MediaPicker.Default.CapturePhotoAsync();
        return result?.FullPath;
    }

    public async Task<string?> PickPhotoAsync()
    {
        var results = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions { SelectionLimit = 1 });
        return results.Count > 0 ? results[0].FullPath : null;
    }
}
