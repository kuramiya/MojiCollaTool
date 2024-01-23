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
    public class BrushToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush? brush = value as SolidColorBrush;

            if (brush != null)
            {
                return $"#{brush.Color.A.ToString("X2")}{brush.Color.R.ToString("X2")}{brush.Color.G.ToString("X2")}{brush.Color.B.ToString("X2")}";
            }
            else
            {
                return "error";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
