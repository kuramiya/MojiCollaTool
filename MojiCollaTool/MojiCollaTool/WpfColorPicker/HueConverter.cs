using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfColorPicker
{
    public class HueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double hue = (((double)value) / 100) * 360;
            byte max = 255;
            byte min = 0;
            byte r = 0;
            byte g = 0;
            byte b = 0;

            if (hue <= 60)
            {
                r = max;
                g = (byte)((hue / 60) * max);
                b = min;
            }
            else if (hue <= 120)
            {
                r = (byte)(((120 - hue) / 60) * max);
                g = max;
                b = min;
            }
            else if (hue <= 180)
            {
                r = min;
                g = max;
                b = (byte)(((hue - 120) / 60) * max);
            }
            else if (hue <= 240)
            {
                r = min;
                g = (byte)(((240 - hue) / 60) * max);
                b = max;
            }
            else if (hue <= 300)
            {
                r = (byte)(((hue - 240) / 60) * max);
                g = min;
                b = max;
            }
            else
            {
                r = max;
                g = min;
                b = (byte)(((360 - hue) / 60) * max);
            }

            return new SolidColorBrush(Color.FromRgb(r,g,b));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
