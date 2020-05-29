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

namespace WeatherAPITest.ViewModel
{
    class WeatherViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WeatherModel Model{ get; set; }

        public WeatherViewModel()
        {
            var client = new RestClient("http://api.openweathermap.org/data/2.5/");
            var request = new RestRequest("weather?q=Amstetten,AT&appid=a8dfc048e5d6fc6a63616f39cb2c312f", DataFormat.Json);
            var response = client.Get(request);

            Console.WriteLine(JObject.Parse(response.Content));
            Model = WeatherModel.FromJson(JObject.Parse(response.Content));
        }
    }
}
