using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderCompare.Converters;

/// <summary>
/// Converts a boolean to Visibility, inverting the logic.
/// Returns Visible when false, Collapsed when true.
/// </summary>
public class InvertBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && !boolValue)
            return Visibility.Visible;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
            return visibility != Visibility.Visible;

        return true;
    }
}
