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

        return result.Success ? ExtractPlateCandidate(result) : null;
    }

    private static string? ExtractPlateCandidate(OcrResult result)
    {
        var candidates = result.Lines
            .Select(Normalize)
            .Where(line => line.Length is >= 4 and <= 8)
            .OrderByDescending(line => line.Length)
            .ToList();

        if (candidates.Count > 0)
        {
            return candidates[0];
        }

        var fallback = Normalize(result.AllText);
        return fallback.Length > 0 ? fallback : null;
    }

    private static string Normalize(string text)
        => new string(text.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
}
