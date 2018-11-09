using System;
using System.Globalization;
using Xamarin.Forms;

namespace Joystick.ValueConverters
{
    public class DummyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
