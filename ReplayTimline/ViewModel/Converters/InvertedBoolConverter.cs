using System;
using System.Globalization;
using System.Windows.Data;


namespace ReplayTimeline
{
	public class InvertedBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool inputBool = (bool)value;

			return !inputBool;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool inputBool = (bool)value;

			return !inputBool;
		}
	}
}
