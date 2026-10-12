namespace Plated.Core.Services;

public interface IInterstitialAdService
{
    /// <summary>
    /// Shows a full-screen ad if ads are on and one is ready, completing when the person closes it.
    /// Completes immediately (never throws) when ads are removed, nothing is loaded, or the ad fails.
    /// </summary>
    Task ShowIfReadyAsync();
}
