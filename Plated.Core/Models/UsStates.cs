namespace Plated.Core.Models;

public static class UsStates
{
    public static readonly IReadOnlyList<string> Codes = new[]
    {
        "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA",
        "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
        "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
        "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
        "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY",
        "DC",
    };

    private static readonly Dictionary<string, string> Names = new()
    {
        ["AL"] = "Alabama", ["AK"] = "Alaska", ["AZ"] = "Arizona", ["AR"] = "Arkansas", ["CA"] = "California",
        ["CO"] = "Colorado", ["CT"] = "Connecticut", ["DE"] = "Delaware", ["FL"] = "Florida", ["GA"] = "Georgia",
        ["HI"] = "Hawaii", ["ID"] = "Idaho", ["IL"] = "Illinois", ["IN"] = "Indiana", ["IA"] = "Iowa",
        ["KS"] = "Kansas", ["KY"] = "Kentucky", ["LA"] = "Louisiana", ["ME"] = "Maine", ["MD"] = "Maryland",
        ["MA"] = "Massachusetts", ["MI"] = "Michigan", ["MN"] = "Minnesota", ["MS"] = "Mississippi", ["MO"] = "Missouri",
        ["MT"] = "Montana", ["NE"] = "Nebraska", ["NV"] = "Nevada", ["NH"] = "New Hampshire", ["NJ"] = "New Jersey",
        ["NM"] = "New Mexico", ["NY"] = "New York", ["NC"] = "North Carolina", ["ND"] = "North Dakota", ["OH"] = "Ohio",
        ["OK"] = "Oklahoma", ["OR"] = "Oregon", ["PA"] = "Pennsylvania", ["RI"] = "Rhode Island", ["SC"] = "South Carolina",
        ["SD"] = "South Dakota", ["TN"] = "Tennessee", ["TX"] = "Texas", ["UT"] = "Utah", ["VT"] = "Vermont",
        ["VA"] = "Virginia", ["WA"] = "Washington", ["WV"] = "West Virginia", ["WI"] = "Wisconsin", ["WY"] = "Wyoming",
        ["DC"] = "District of Columbia",
    };

    /// <summary>Full state name for a two-letter code (falls back to the code itself).</summary>
    public static string NameOf(string? code)
        => code is not null && Names.TryGetValue(code.ToUpperInvariant(), out var name) ? name : code ?? string.Empty;

    /// <summary>Two-letter code for a full state name (falls back to the text itself).</summary>
    public static string CodeOf(string? name)
        => Names.FirstOrDefault(kv => string.Equals(kv.Value, name, StringComparison.OrdinalIgnoreCase)).Key ?? name ?? string.Empty;

    /// <summary>Full state names in the same order as <see cref="Codes"/>.</summary>
    public static IReadOnlyList<string> AllNames => Codes.Select(NameOf).ToList();
}
