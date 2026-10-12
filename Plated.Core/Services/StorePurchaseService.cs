using Plugin.InAppBilling;

namespace Plated.Core.Services;

/// <summary>
/// One-time, non-consumable "Remove ads" purchase through Google Play Billing / StoreKit.
/// Create a product with ID <see cref="RemoveAdsProductId"/> in Play Console and App Store Connect.
/// </summary>
public class StorePurchaseService : IPurchaseService
{
    public const string RemoveAdsProductId = "remove_ads";

    public StorePurchaseService(AdsState adsState)
    {
        AdsState = adsState;
    }

    public AdsState AdsState { get; }

    public async Task RefreshAsync()
    {
        try
        {
            if (await OwnsRemoveAdsAsync())
            {
                AdsState.AdsRemoved = true;
            }
        }
        catch (Exception ex)
        {
            // Offline or store unavailable: keep whatever we cached last time.
            System.Diagnostics.Debug.WriteLine($"[Plated] Purchase refresh failed: {ex.Message}");
        }
    }

    public async Task<string?> GetPriceAsync()
    {
        var billing = CrossInAppBilling.Current;
        try
        {
            if (!await billing.ConnectAsync())
            {
                return null;
            }

            var products = await billing.GetProductInfoAsync(ItemType.InAppPurchase, new[] { RemoveAdsProductId });
            return products?.FirstOrDefault()?.LocalizedPrice;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Price lookup failed: {ex.Message}");
            return null;
        }
        finally
        {
            await DisconnectQuietlyAsync(billing);
        }
    }

    public async Task<PurchaseOutcome> PurchaseRemoveAdsAsync()
    {
        var billing = CrossInAppBilling.Current;
        try
        {
            if (!await billing.ConnectAsync())
            {
                return PurchaseOutcome.Failed;
            }

            var purchase = await billing.PurchaseAsync(RemoveAdsProductId, ItemType.InAppPurchase);
            switch (purchase?.State)
            {
                case PurchaseState.Purchased:
                case PurchaseState.Restored:
                    await AcknowledgeAsync(billing, purchase);
                    AdsState.AdsRemoved = true;
                    return PurchaseOutcome.Purchased;

                case PurchaseState.PaymentPending:
                case PurchaseState.Deferred:
                    return PurchaseOutcome.Pending;

                case PurchaseState.Canceled:
                    return PurchaseOutcome.Cancelled;

                default:
                    return PurchaseOutcome.Failed;
            }
        }
        catch (InAppBillingPurchaseException ex) when (ex.PurchaseError == PurchaseError.UserCancelled)
        {
            return PurchaseOutcome.Cancelled;
        }
        catch (InAppBillingPurchaseException ex) when (ex.PurchaseError == PurchaseError.AlreadyOwned)
        {
            // They bought it before (another phone, reinstall): just unlock it.
            AdsState.AdsRemoved = true;
            return PurchaseOutcome.Purchased;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Purchase failed: {ex}");
            return PurchaseOutcome.Failed;
        }
        finally
        {
            await DisconnectQuietlyAsync(billing);
        }
    }

    public async Task<bool> RestorePurchasesAsync()
    {
        try
        {
            if (await OwnsRemoveAdsAsync())
            {
                AdsState.AdsRemoved = true;
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Restore failed: {ex.Message}");
        }

        return false;
    }

    private static async Task<bool> OwnsRemoveAdsAsync()
    {
        var billing = CrossInAppBilling.Current;
        try
        {
            if (!await billing.ConnectAsync())
            {
                return false;
            }

            var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
            var owned = purchases?.FirstOrDefault(p =>
                p.ProductId == RemoveAdsProductId &&
                p.State is PurchaseState.Purchased or PurchaseState.Restored);

            if (owned is null)
            {
                return false;
            }

            await AcknowledgeAsync(billing, owned);
            return true;
        }
        finally
        {
            await DisconnectQuietlyAsync(billing);
        }
    }

    /// <summary>Google refunds purchases that aren't acknowledged within three days.</summary>
    private static async Task AcknowledgeAsync(IInAppBilling billing, InAppBillingPurchase purchase)
    {
        if (purchase.IsAcknowledged == true)
        {
            return;
        }

        try
        {
            await billing.FinalizePurchaseAsync(new[] { purchase.TransactionIdentifier });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Plated] Acknowledge failed: {ex.Message}");
        }
    }

    private static async Task DisconnectQuietlyAsync(IInAppBilling billing)
    {
        try
        {
            await billing.DisconnectAsync();
        }
        catch (Exception)
        {
            // Nothing useful to do.
        }
    }
}
