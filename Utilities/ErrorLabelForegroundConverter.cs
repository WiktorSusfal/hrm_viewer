using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace HRM_Viewer.Utilities
{
    class ErrorLabelForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c;
            int v = (int)value;

            // If it is errror.
            if (v == -1)
                c = Color.FromRgb(255, 255, 255);
            // If it is success.
            else if (v == 1)
                c = Color.FromRgb(0, 0, 0);
            else
                c = Color.FromArgb(0, 0, 0, 0);

            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
