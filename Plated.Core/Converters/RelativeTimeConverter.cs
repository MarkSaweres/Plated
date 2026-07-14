using System.Globalization;
using Microsoft.Maui.Controls;

namespace Plated.Core.Converters;

public class RelativeTimeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DateTimeOffset timestamp)
        {
            return string.Empty;
        }

        var elapsed = DateTimeOffset.UtcNow - timestamp;

        if (elapsed < TimeSpan.FromMinutes(1))
        {
            return "just now";
        }
        if (elapsed < TimeSpan.FromHours(1))
        {
            return $"{(int)elapsed.TotalMinutes}m ago";
        }
        if (elapsed < TimeSpan.FromDays(1))
        {
            return $"{(int)elapsed.TotalHours}h ago";
        }
        if (elapsed < TimeSpan.FromDays(30))
        {
            return $"{(int)elapsed.TotalDays}d ago";
        }

        return timestamp.LocalDateTime.ToString("MMM d, yyyy", culture);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
