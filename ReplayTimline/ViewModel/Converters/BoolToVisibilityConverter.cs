using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace ReplayTimeline
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool inputBool = (bool)value;

			return inputBool == true ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Visibility visibilityInput = (Visibility)value;

			return visibilityInput == Visibility.Visible ? true : false;
		}
	}
}
