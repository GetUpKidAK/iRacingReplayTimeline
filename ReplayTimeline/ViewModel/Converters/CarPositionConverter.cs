using System;
using System.Globalization;
using System.Windows.Data;


namespace iRacingReplayDirector
{
	class CarPositionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int inputValue = (int)value;

			if (inputValue == 999)
			{
				return "-";
			}

			return inputValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
