using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FolderCompare.Converters;

/// <summary>
/// Converts a boolean (IsDirectory) to FontWeight.
/// Returns Bold for directories, Normal for files.
/// </summary>
public class FolderFontWeightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isDirectory && isDirectory)
            return FontWeights.Bold;

        return FontWeights.Normal;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
