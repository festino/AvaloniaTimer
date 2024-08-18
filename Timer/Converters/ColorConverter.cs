using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace Timer.Converters;

public class ColorConverter : IValueConverter
{
	public static readonly ColorConverter Instance = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not Color color)
			return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

		return new SolidColorBrush(color);
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not SolidColorBrush brush)
			return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

		return brush.Color;
	}
}
