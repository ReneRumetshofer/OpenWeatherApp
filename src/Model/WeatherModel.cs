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
        public int? CityID { get; set; }
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
        public double? Visibility { get; set; }
        public double? RainOneHour { get; set; } // in mm/h
        public double? SnowOneHour { get; set; } // in mm/h
        public double WindSpeed { get; set; }
        public double? WindDegree { get; set; }
        public double CloudsAll { get; set; }
        
        public DateTime DataTimestamp { get; set; }
        public DateTime SunriseTimestamp { get; set; }
        public DateTime SunsetTimestamp { get; set; }

        public const double CELSIUS_IN_KELVIN = 273.15;

        public static WeatherModel FromJson(JObject json)
        {
            return new WeatherModel() {
                Latitude = double.Parse(json["coord"]["lat"].ToString()),
                Longitude = double.Parse(json["coord"]["lon"].ToString()),
                Name = json["name"].ToString(),
                CityID = json["sys"]["id"] != null ? int.Parse(json["sys"]["id"].ToString()) : default(int?),
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
                Visibility = json["visibility"] != null ? double.Parse(json["visibility"].ToString()) : default(double?),
                RainOneHour = json["rain"]?["1h"] != null ? double.Parse(json["rain"]["1h"].ToString()) : default(double?),
                SnowOneHour = json["snow"]?["1h"] != null ? double.Parse(json["snow"]["1h"].ToString()) : default(double?),
                WindSpeed = double.Parse(json["wind"]["speed"].ToString()),
                WindDegree = json["wind"]["deg"] != null ? double.Parse(json["wind"]["deg"].ToString()) : default(double?),
                CloudsAll = double.Parse(json["clouds"]["all"].ToString()),

                DataTimestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(json["dt"].ToString())).DateTime,
                SunriseTimestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(json["sys"]["sunrise"].ToString())).DateTime,
                SunsetTimestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(json["sys"]["sunset"].ToString())).DateTime
            };
        }

        // For example from Kelvin to °C
        public void AdjustUnits()
        {
            Temperature = Temperature - CELSIUS_IN_KELVIN;
            FeltTemperature = FeltTemperature - CELSIUS_IN_KELVIN;
            MinTemperature = MinTemperature - CELSIUS_IN_KELVIN;
            MaxTemperature = MaxTemperature - CELSIUS_IN_KELVIN;
            Pressure = Pressure / 1000; // mbar to bars
            Humidity = Humidity / 100; // normalization
            CloudsAll = CloudsAll / 100;
        }

    }
}
