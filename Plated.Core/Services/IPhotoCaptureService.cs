namespace Plated.Core.Services;

public interface IPhotoCaptureService
{
    bool IsCaptureSupported { get; }

    Task<string?> CapturePhotoAsync();

    Task<string?> PickPhotoAsync();
}
