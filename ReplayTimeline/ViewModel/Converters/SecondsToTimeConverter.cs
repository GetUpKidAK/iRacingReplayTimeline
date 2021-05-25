using System;
using System.Globalization;
using System.Windows.Data;


namespace iRacingReplayDirector
{
	class SecondsToTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double inputValue = (double)value;

			TimeSpan time = TimeSpan.FromSeconds(inputValue);

			//here backslash is must to tell that colon is not the part of format, it just a character that we want in output
			//string str = time.ToString(@"hh\:mm\:ss\:fff");
			string str = time.ToString(@"hh\:mm\:ss");

			return str;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string inputValue = (string)value;

			TimeSpan time = TimeSpan.Parse(inputValue);

			return time.TotalSeconds;
		}
	}
}
