using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Input;
using WeatherAPITest.Model;
using System.Configuration;
using System.Net;
using System.Threading;
using System.Windows;
using WeatherAPITest.View;
using System.Windows.Threading;

namespace WeatherAPITest.ViewModel
{
    class WeatherViewModel : INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        public string CityNameView { get; set; } = string.Empty; // View binds to these properties to not interfere with the refreshing task
        public string CountryTagView { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CountryTag { get; set; } = string.Empty;

        public string ErrorText { get; set; } = string.Empty;
        public bool ErrorTextEnabled => ErrorText != null && ErrorText != string.Empty && ErrorText != "";
        public WeatherModel Model{ get; set; }
        public bool ModelLoaded => Model != null;

        private Task refreshTask;
        #endregion

        #region Commands
        public ICommand LoadCommand => new RelayCommand((e) =>
        {
            CityName = CityNameView;
            CountryTag = CountryTagView;

            LoadCity();
        },
        (c) => true);
        #endregion

        /// <summary>
        /// Initializes the window's ViewModel
        /// </summary>
        public WeatherViewModel()
        {
            CityName = ConfigurationManager.AppSettings["CityName"];
            CountryTag = ConfigurationManager.AppSettings["CountryTag"];

            // Initialize refresh task
            refreshTask = new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    if (ModelLoaded && !ErrorTextEnabled)
                        LoadCity();
                }
            });

            // TODO: Make asynchronous
            // Check if an API key is set in Config
            //if (ConfigurationManager.AppSettings["API key"] == null)
            //{
            //    APIKeyDialog akd = new APIKeyDialog(string.Empty);
            //    akd.ShowDialog();
            //}

            // If city and country tag is set, then load
            if (CityName != null && CountryTag != null)
            {
                CityNameView = CityName;
                CountryTagView = CountryTag;
                LoadCity();
            }
        }

        /// <summary>
        /// Queries the API for the city set in CityName and CountryTag properties.
        /// </summary>
        private void LoadCity()
        {
            ErrorText = string.Empty;

            string apiKey = ConfigurationManager.AppSettings["API key"];
            if (apiKey == null)
            {
                // Prompt for key
                APIKeyDialog akd = new APIKeyDialog(string.Empty);
                akd.ShowDialog();

                // Check if set after prompt
                apiKey = ConfigurationManager.AppSettings["API key"];
                if (apiKey == null || apiKey.Length == 0)
                    ErrorText = "No API key set in App.config!";
                else
                    LoadCity();
            }
            else
            {
                var client = new RestClient("http://api.openweathermap.org/data/2.5/");
                var request = new RestRequest($"weather?q={CityName},{CountryTag}&appid={apiKey}", RestSharp.DataFormat.Json);
                var response = client.Get(request);

                // City has been found (HTTP 200)
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseJson = JObject.Parse(response.Content);
                    //Console.WriteLine(JObject.Parse(response.Content));

                    // Parsing can throw exception, because not all cities have complete json attribute sets
                    try
                    {
                        Model = WeatherModel.FromJson(JObject.Parse(response.Content));
                        Model.AdjustUnits();

                        // Save city into config
                        AddConfigurationEntry("CityName", CityName);
                        AddConfigurationEntry("CountryTag", CountryTag);

                        // Start refresher thread
                        if (!refreshTask.Status.Equals(TaskStatus.Running))
                            refreshTask.Start();
                    }
                    catch (Exception)
                    {
                        ErrorText = "Incomplete API response for this city!";
                        MessageBox.Show($"The API response of the city {CityName}, {CountryTag} is incomplete and can't be parsed properly.", "Incomplete city response",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                // City not found
                else if(response.StatusCode == HttpStatusCode.NotFound)
                {
                    ErrorText = "City and/or country could not be found!";
                    Model = null;
                }
                // Invalid API key
                else if(response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ErrorText = "Invalid API key!";

                    MessageBox.Show("The API key in the App.config is invalid! Please change it in the next prompt.", "Invalid API key", MessageBoxButton.OK, MessageBoxImage.Warning);
                    APIKeyDialog akd = new APIKeyDialog(apiKey);
                    akd.ShowDialog();
                }
            }
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
