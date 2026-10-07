using System.Globalization;
using Microsoft.Maui.Controls;

namespace Plated.Core.Converters;

/// <summary>First letter of a name, uppercased, for avatar circles.</summary>
public class InitialConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var name = (value as string)?.Trim();
        return string.IsNullOrEmpty(name) ? "?" : name[..1].ToUpperInvariant();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
