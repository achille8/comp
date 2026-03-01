using System;
using System.Globalization;
using System.Windows.Data;
using FolderCompare.Models;

namespace FolderCompare.Converters;

public class StatusToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComparisonStatus status)
        {
            return status switch
            {
                ComparisonStatus.Identical => "✓",
                ComparisonStatus.Modified => "≠",
                ComparisonStatus.LeftOnly => "←",
                ComparisonStatus.RightOnly => "→",
                ComparisonStatus.DifferentSize => "↔",
                _ => "?",
            };
        }

        return "?";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
