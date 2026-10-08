namespace Plated.Core.Services;

/// <summary>Picks the most plate-like string out of the lines of text an OCR pass found in a photo.</summary>
public static class PlateTextParser
{
    // Text printed on plates that is not the plate number itself.
    private static readonly string[] NoiseFragments =
    {
        "dmv", "gov", "www", "http", ".com", "california", "golden", "state", "registration", "expires",
    };

    private static readonly HashSet<string> StateNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "ALABAMA", "ALASKA", "ARIZONA", "ARKANSAS", "COLORADO", "CONNECTICUT", "DELAWARE", "FLORIDA",
        "GEORGIA", "HAWAII", "IDAHO", "ILLINOIS", "INDIANA", "IOWA", "KANSAS", "KENTUCKY", "LOUISIANA",
        "MAINE", "MARYLAND", "MASSACHUSETTS", "MICHIGAN", "MINNESOTA", "MISSISSIPPI", "MISSOURI",
        "MONTANA", "NEBRASKA", "NEVADA", "NEWHAMPSHIRE", "NEWJERSEY", "NEWMEXICO", "NEWYORK",
        "NORTHCAROLINA", "NORTHDAKOTA", "OHIO", "OKLAHOMA", "OREGON", "PENNSYLVANIA", "RHODEISLAND",
        "SOUTHCAROLINA", "SOUTHDAKOTA", "TENNESSEE", "TEXAS", "UTAH", "VERMONT", "VIRGINIA",
        "WASHINGTON", "WESTVIRGINIA", "WISCONSIN", "WYOMING",
    };

    public static string? Pick(IEnumerable<string> lines)
    {
        string? best = null;
        var bestScore = 0;

        foreach (var line in lines)
        {
            if (IsNoise(line))
            {
                continue;
            }

            var candidate = Normalize(line);
            if (candidate.Length is < 4 or > 8 || StateNames.Contains(candidate))
            {
                continue;
            }

            var score = Score(candidate);
            if (score > bestScore)
            {
                best = candidate;
                bestScore = score;
            }
        }

        return best;
    }

    public static string Normalize(string text)
        => new string(text.Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();

    private static bool IsNoise(string line)
    {
        var lower = line.ToLowerInvariant();
        return NoiseFragments.Any(lower.Contains);
    }

    private static int Score(string candidate)
    {
        var hasDigit = candidate.Any(char.IsDigit);
        var hasLetter = candidate.Any(char.IsLetter);

        var score = hasDigit && hasLetter ? 3 : 1;
        score += candidate.Length switch
        {
            6 or 7 => 2,
            5 or 8 => 1,
            _ => 0,
        };

        return score;
    }
}
