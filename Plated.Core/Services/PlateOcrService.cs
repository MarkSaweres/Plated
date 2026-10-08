using Plugin.Maui.OCR;

namespace Plated.Core.Services;

public class PlateOcrService : IPlateOcrService
{
    private readonly IOcrService _ocrService;

    public PlateOcrService(IOcrService ocrService)
    {
        _ocrService = ocrService;
    }

    public async Task<string?> RecognizePlateNumberAsync(string imageFilePath)
    {
        await _ocrService.InitAsync();

        var imageBytes = await File.ReadAllBytesAsync(imageFilePath);
        var result = await _ocrService.RecognizeTextAsync(imageBytes, tryHard: true);

        return result.Success ? PlateTextParser.Pick(result.Lines) : null;
    }
}
