using System.Globalization;
using System.Windows.Data;
using FolderCompare.Models;

namespace FolderCompare.Converters;

public class DiffTypeToSymbolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DiffLineType type)
        {
            return type switch
            {
                DiffLineType.Unchanged => " ",
                DiffLineType.Deleted => "◄",
                DiffLineType.Added => "►",
                DiffLineType.Modified => "≠",
                _ => " "
            };
        }

        return " ";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
