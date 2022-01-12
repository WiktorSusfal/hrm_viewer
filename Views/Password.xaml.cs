using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HRM_Viewer.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Password.xaml
    /// </summary>
    public partial class Password : UserControl
    {
        public Password()
        {
            InitializeComponent();
        }

        private void SQLConnectionPsswdChanged(object sender, RoutedEventArgs e)
        {
            if (App.Current.MainWindow.DataContext != null)
                ((dynamic)App.Current.MainWindow.DataContext).MStModel.SQLConnPswd = ((PasswordBox)sender).Password;
        }
    }
}
