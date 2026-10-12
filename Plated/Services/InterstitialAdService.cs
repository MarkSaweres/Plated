using Plated.Core.Services;
using Plugin.MauiMtAdmob;

namespace Plated.Services;

/// <summary>
/// Full-screen AdMob ads. One ad is kept preloaded so showing it is instant; the next one is
/// requested as soon as the current one is dismissed.
/// </summary>
public class InterstitialAdService : IInterstitialAdService
{
    // If the ad never reports back (closed or failed), don't leave the person stuck on a spinner.
    private static readonly TimeSpan ShowTimeout = TimeSpan.FromMinutes(2);

    private readonly AdsState _adsState;
    private TaskCompletionSource? _showing;

    public InterstitialAdService(AdsState adsState)
    {
        _adsState = adsState;

        try
        {
            var ads = CrossMauiMTAdmob.Current;
            ads.OnInterstitialClosed += (_, _) => OnFinished();
            ads.OnInterstitialFailedToShow += (_, _) => OnFinished();
            ads.OnInterstitialFailedToLoad += (_, _) => System.Diagnostics.Debug.WriteLine("[Plated] Interstitial failed to load");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Interstitial setup failed: {ex.Message}");
        }

        Preload();
    }

    public async Task ShowIfReadyAsync()
    {
        if (!_adsState.ShowAds || _showing is not null)
        {
            return;
        }

        try
        {
            var ads = CrossMauiMTAdmob.Current;
            if (!ads.IsInterstitialLoaded())
            {
                Preload();
                return;
            }

            _showing = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            ads.ShowInterstitial();
            await Task.WhenAny(_showing.Task, Task.Delay(ShowTimeout));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Interstitial failed: {ex.Message}");
        }
        finally
        {
            _showing = null;
        }
    }

    private void OnFinished()
    {
        _showing?.TrySetResult();
        Preload();
    }

    private static void Preload()
    {
        try
        {
            var ads = CrossMauiMTAdmob.Current;
            if (!ads.IsInterstitialLoaded())
            {
                ads.LoadInterstitial(AdConfig.InterstitialAdUnitId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Interstitial preload failed: {ex.Message}");
        }
    }
}
