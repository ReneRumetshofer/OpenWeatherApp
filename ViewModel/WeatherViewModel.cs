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

namespace WeatherAPITest.ViewModel
{
    class WeatherViewModel : INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        public string CityName { get; set; } = string.Empty;
        public string CountryTag { get; set; } = string.Empty;
        public string ErrorText { get; set; } = string.Empty;
        public bool ErrorTextEnabled => ErrorText != null && ErrorText != string.Empty && ErrorText != "";
        public WeatherModel Model{ get; set; }
        public bool ModelLoaded => Model != null;
        #endregion

        #region Commands
        public ICommand LoadCommand => new RelayCommand((e) =>
        {
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

            // If city and country tag isset, then load
            if(CityName != null && CountryTag != null)
            {
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
                ErrorText = "No API key set in App.config!";
            }
            else
            {
                var client = new RestClient("http://api.openweathermap.org/data/2.5/");
                var request = new RestRequest($"weather?q={CityName},{CountryTag}&appid={apiKey}", DataFormat.Json);
                var response = client.Get(request);

                // City has been found (HTTP 200)
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    JObject responseJson = JObject.Parse(response.Content);
                    //Console.WriteLine(JObject.Parse(response.Content));
                    Model = WeatherModel.FromJson(JObject.Parse(response.Content));
                    Model.AdjustUnits();

                    // Save city into config
                    AddConfigurationEntry("CityName", CityName);
                    AddConfigurationEntry("CountryTag", CountryTag);
                }
                // City not found
                else
                {
                    ErrorText = "City and/or country could not be found!";
                    Model = null;
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
