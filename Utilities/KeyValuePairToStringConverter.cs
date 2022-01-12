using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HRM_Viewer.Utilities
{
    class KeyValuePairToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> result = new List<string>();
            var v = (ObservableCollection<KeyValuePair<string, string>>)value;

            if (value == null)
                return new ObservableCollection<KeyValuePair<string, string>>();
           
            foreach (KeyValuePair<string,string> kp in v)
            {
                result.Add(kp.Key + " - " + kp.Value);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
    
}
