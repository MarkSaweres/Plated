using System.Text.Json;
using Microsoft.Maui.Storage;

namespace Plated.Core.Services;

/// <summary>Remembers the last few plates looked up on this device.</summary>
public class RecentPlatesService : IRecentPlatesService
{
    private const string PreferenceKey = "recent_plates";
    private const int MaxItems = 8;

    public IReadOnlyList<RecentPlate> Load()
    {
        try
        {
            var json = Preferences.Default.Get(PreferenceKey, string.Empty);
            return string.IsNullOrEmpty(json)
                ? new List<RecentPlate>()
                : JsonSerializer.Deserialize<List<RecentPlate>>(json) ?? new List<RecentPlate>();
        }
        catch (Exception)
        {
            return new List<RecentPlate>();
        }
    }

    public IReadOnlyList<RecentPlate> Add(string state, string number)
    {
        var list = Load().ToList();
        list.RemoveAll(p => p.State == state && string.Equals(p.Number, number, StringComparison.OrdinalIgnoreCase));
        list.Insert(0, new RecentPlate(state, number.ToUpperInvariant()));
        if (list.Count > MaxItems)
        {
            list.RemoveRange(MaxItems, list.Count - MaxItems);
        }

        try
        {
            Preferences.Default.Set(PreferenceKey, JsonSerializer.Serialize(list));
        }
        catch (Exception)
        {
            // History is a convenience only.
        }

        return list;
    }
}
