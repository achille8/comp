using System;
using System.Globalization;
using System.Windows.Data;

namespace FolderCompare.Converters;

public class FileSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not long size)
            return "—";

        return size switch
        {
            < 1024L => $"{size} B",
            < 1048576L => $"{size / 1024.0:F1} KB",
            < 1073741824L => $"{size / 1048576.0:F1} MB",
            _ => $"{size / 1073741824.0:F2} GB",
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
