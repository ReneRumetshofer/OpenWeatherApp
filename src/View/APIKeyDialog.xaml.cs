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
using System.Windows.Shapes;
using WeatherAPITest.ViewModel;

namespace WeatherAPITest.View
{
    /// <summary>
    /// Interaktionslogik für APIKeyDialog.xaml
    /// </summary>
    public partial class APIKeyDialog : Window
    {
        public APIKeyDialog(string existingAPIKey)
        {
            APIKeyDialogViewModel vm = new APIKeyDialogViewModel(existingAPIKey);
            vm.Close += (s, e) => Close(); // Add handler for ViewModel Close event, so that window closes when event fires
            DataContext = vm;

            InitializeComponent();
        }
    }
}
