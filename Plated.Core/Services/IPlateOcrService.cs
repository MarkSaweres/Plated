namespace Plated.Core.Services;

public interface IPlateOcrService
{
    Task<string?> RecognizePlateNumberAsync(string imageFilePath);
}
