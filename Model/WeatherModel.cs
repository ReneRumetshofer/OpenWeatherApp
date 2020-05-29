using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAPITest.Model
{
    public class WeatherModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public int CityID { get; set; }
        public string CountryTag { get; set; }

        public int WeatherID { get; set; }
        public string WeatherMain { get; set; }        
        public string WeatherDescription { get; set; }
        public double Temperature { get; set; }
        public double FeltTemperature { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }
        public double Visibility { get; set; }
        public double WindSpeed { get; set; }
        public double WindDegree { get; set; }
        public double CloudsAll { get; set; }
        
        public long DataTimestamp { get; set; }
        public long SunriseTimestamp { get; set; }
        public long SunsetTimestamp { get; set; }

        public static WeatherModel FromJson(JObject json)
        {
            return new WeatherModel() { 
                Latitude = double.Parse(json["coord"]["lat"].ToString()),
                Longitude = double.Parse(json["coord"]["lon"].ToString()),
                Name = json["name"].ToString(),
                CityID = int.Parse(json["sys"]["id"].ToString()),
                CountryTag = json["sys"]["country"].ToString(),

                WeatherID = int.Parse(json["weather"][0]["id"].ToString()),
                WeatherMain = json["weather"][0]["main"].ToString(),
                WeatherDescription = json["weather"][0]["description"].ToString(),
                Temperature = double.Parse(json["main"]["temp"].ToString()),
                FeltTemperature = double.Parse(json["main"]["feels_like"].ToString()),
                MinTemperature = double.Parse(json["main"]["temp_min"].ToString()),
                MaxTemperature = double.Parse(json["main"]["temp_max"].ToString()),
                Pressure = double.Parse(json["main"]["pressure"].ToString()),
                Humidity = double.Parse(json["main"]["humidity"].ToString()),
                Visibility = double.Parse(json["visibility"].ToString()),
                WindSpeed = double.Parse(json["wind"]["speed"].ToString()),
                WindDegree = double.Parse(json["wind"]["deg"].ToString()),
                CloudsAll = double.Parse(json["clouds"]["all"].ToString()),

                DataTimestamp = long.Parse(json["dt"].ToString()),
                SunriseTimestamp = long.Parse(json["sys"]["sunrise"].ToString()),
                SunsetTimestamp = long.Parse(json["sys"]["sunset"].ToString())
            };
        }

    }
}
