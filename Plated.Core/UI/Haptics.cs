using Microsoft.Maui.Devices;

namespace Plated.Core.UI;

public static class Haptics
{
    /// <summary>A light tap on the device, if it supports one.</summary>
    public static void Tap()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (Exception)
        {
            // Not supported on this device or emulator.
        }
    }
}
