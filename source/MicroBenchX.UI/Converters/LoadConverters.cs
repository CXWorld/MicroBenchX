using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MicroBenchX.UI.Converters;

/// <summary>
/// Converts load percentage (0-100) to width for load bar display.
/// The bar width is based on a max width of approximately 80 pixels (core type badge + button width).
/// </summary>
public class LoadToWidthConverter : IValueConverter
{
    private const double MaxWidth = 80.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double load)
        {
            // Clamp load to 0-100 range
            load = Math.Max(0, Math.Min(100, load));
            return load / 100.0 * MaxWidth;
        }
        return 0.0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Returns true if load is above threshold (default 80%), indicating high load.
/// </summary>
public class LoadToHighConverter : IValueConverter
{
    private const double HighLoadThreshold = 80.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double load)
        {
            return load > HighLoadThreshold;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Multi-value converter that calculates load bar width as a percentage of the container width.
/// Values[0] = Load (0-100), Values[1] = Container ActualWidth
/// </summary>
public class LoadToPercentWidthConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 &&
            values[0] is double load &&
            values[1] is double containerWidth &&
            containerWidth > 0)
        {
            // Clamp load to 0-100 range
            load = Math.Max(0, Math.Min(100, load));
            return load / 100.0 * containerWidth;
        }
        return 0.0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
