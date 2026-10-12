using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Storage;

namespace Plated.Core.Services;

/// <summary>
/// Whether ads should be shown. The "ads removed" flag is cached on the device so the app doesn't
/// flash ads (or need the store) at launch; <see cref="IPurchaseService.RefreshAsync"/> keeps it honest.
/// </summary>
public partial class AdsState : ObservableObject
{
    private const string PreferenceKey = "ads_removed";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowAds))]
    private bool adsRemoved = ReadCachedValue();

    public bool ShowAds => !AdsRemoved;

    partial void OnAdsRemovedChanged(bool value)
    {
        try
        {
            Preferences.Default.Set(PreferenceKey, value);
        }
        catch (Exception)
        {
            // The cache is a convenience only.
        }
    }

    private static bool ReadCachedValue()
    {
        try
        {
            return Preferences.Default.Get(PreferenceKey, false);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
