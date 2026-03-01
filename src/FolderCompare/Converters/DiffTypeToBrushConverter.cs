using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FolderCompare.Models;

namespace FolderCompare.Converters;

public class DiffTypeToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush TransparentBrush = Brushes.Transparent;
    private static readonly SolidColorBrush DeletedBrush = new((Color)ColorConverter.ConvertFromString("#FFCDD2"));
    private static readonly SolidColorBrush AddedBrush = new((Color)ColorConverter.ConvertFromString("#C8E6C9"));
    private static readonly SolidColorBrush ModifiedBrush = new((Color)ColorConverter.ConvertFromString("#FFF9C4"));

    static DiffTypeToBrushConverter()
    {
        DeletedBrush.Freeze();
        AddedBrush.Freeze();
        ModifiedBrush.Freeze();
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DiffLineType type)
        {
            return type switch
            {
                DiffLineType.Unchanged => TransparentBrush,
                DiffLineType.Deleted => DeletedBrush,
                DiffLineType.Added => AddedBrush,
                DiffLineType.Modified => ModifiedBrush,
                _ => TransparentBrush
            };
        }

        return TransparentBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
