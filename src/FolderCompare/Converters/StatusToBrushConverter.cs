using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FolderCompare.Models;

namespace FolderCompare.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ComparisonStatus status)
        {
            return status switch
            {
                ComparisonStatus.Identical => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9")),
                ComparisonStatus.Modified => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF9C4")),
                ComparisonStatus.LeftOnly => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCDD2")),
                ComparisonStatus.RightOnly => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BBDEFB")),
                ComparisonStatus.DifferentSize => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE0B2")),
                _ => Brushes.Transparent,
            };
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
