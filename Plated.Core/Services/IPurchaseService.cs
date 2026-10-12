namespace Plated.Core.Services;

public enum PurchaseOutcome
{
    /// <summary>The purchase (or a previous one) went through and ads are now removed.</summary>
    Purchased,

    /// <summary>The person backed out of the store dialog.</summary>
    Cancelled,

    /// <summary>The store accepted the order but payment hasn't cleared yet.</summary>
    Pending,

    /// <summary>The store isn't available or the purchase failed.</summary>
    Failed,
}

public interface IPurchaseService
{
    /// <summary>Shared ad visibility flag that the UI binds to.</summary>
    AdsState AdsState { get; }

    /// <summary>Checks the store for an existing "Remove ads" purchase. Safe to call at any time; never throws.</summary>
    Task RefreshAsync();

    /// <summary>The localized price (for example "$2.99"), or null if the store can't be reached.</summary>
    Task<string?> GetPriceAsync();

    Task<PurchaseOutcome> PurchaseRemoveAdsAsync();

    /// <summary>Re-applies a previous purchase, for example after a reinstall or on a new phone.</summary>
    Task<bool> RestorePurchasesAsync();
}
