using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WeatherAPITest.ViewModel
{
    class APIKeyDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string APIKey { get; set; } = string.Empty;

        public ICommand SetKeyCommand => new RelayCommand((e) =>
        {
            AddConfigurationEntry("API key", APIKey);
            Close(this, new EventArgs());
        },
        (c) => APIKey.Length > 0);

        public event EventHandler Close;

        public APIKeyDialogViewModel(string existingAPIKey)
        {
            APIKey = existingAPIKey;
        }

        /// <summary>
        /// Adds an entry into the "appSettings" section in App.config.
        /// </summary>
        /// <param name="key">Key of config entry</param>
        /// <param name="value">Value of config entry</param>
        private void AddConfigurationEntry(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
