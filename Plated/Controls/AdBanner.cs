using Microsoft.Extensions.DependencyInjection;
using Plated.Core.Services;
using Plugin.MauiMtAdmob.Controls;
using Plugin.MauiMtAdmob.Extra;

namespace Plated.Controls;

/// <summary>
/// A banner ad that only exists while ads are on. Drop it at the bottom of a page; it collapses
/// and tears the ad down as soon as the "Remove ads" purchase goes through.
/// </summary>
public class AdBanner : ContentView
{
    private AdsState? _adsState;

    public AdBanner()
    {
        IsVisible = false;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (_adsState is not null)
        {
            _adsState.PropertyChanged -= OnAdsStateChanged;
            _adsState = null;
        }

        if (Handler?.MauiContext?.Services is { } services)
        {
            _adsState = services.GetRequiredService<AdsState>();
            _adsState.PropertyChanged += OnAdsStateChanged;
        }

        Refresh();
    }

    private void OnAdsStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => Dispatcher.Dispatch(Refresh);

    private void Refresh()
    {
        var show = _adsState?.ShowAds == true;
        if (show && Content is null)
        {
            Content = new MTAdView
            {
                AdsId = AdConfig.BannerAdUnitId,
                AdSize = BannerSize.Banner,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Center,
            };
        }
        else if (!show)
        {
            Content = null;
        }

        IsVisible = show;
    }
}
