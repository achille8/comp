using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FolderCompare.Converters;

/// <summary>
/// Converts a depth value to a left margin (Thickness) for indentation.
/// </summary>
public class DepthToMarginConverter : IValueConverter
{
    /// <summary>
    /// The indentation per depth level (in pixels).
    /// </summary>
    public double IndentPerLevel { get; set; } = 20.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int depth)
        {
            return new Thickness(depth * IndentPerLevel, 0, 0, 0);
        }
        return new Thickness(0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("DepthToMarginConverter does not support ConvertBack.");
    }
}
