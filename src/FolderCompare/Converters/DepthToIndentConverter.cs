namespace FolderCompare.Converters;

using System;
using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converts a depth value to an indentation width (in pixels).
/// Each depth level adds a fixed indentation (e.g., 20 pixels).
/// </summary>
public class DepthToIndentConverter : IValueConverter
{
    /// <summary>
    /// The indentation per depth level (in pixels).
    /// </summary>
    public double IndentPerLevel { get; set; } = 20.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int depth)
        {
            return depth * IndentPerLevel;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("DepthToIndentConverter does not support ConvertBack.");
    }
}
