using Android.Gms.Common.Apis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TripPlanner.Model;


namespace TripPlanner.Services
{
    public class WeatherApiService
    {

        private static   HttpClient _httpClient = new HttpClient();
        private static  string _apiKey = "";                            //Insert Keys.txt file in Resouces/raw folder and put your own API key in 2nd line of the file

        public WeatherApiService()
        {
            
        }

        static async void Init()
        {
            if (_apiKey.Length != 0)
                return;
            try
            {
                using var fileStream = await FileSystem.OpenAppPackageFileAsync("Keys.txt");
                using var reader = new StreamReader(fileStream);

                _apiKey =  reader.ReadToEndAsync().Result;
                _apiKey = _apiKey.Split("\n")[1];
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        public static async Task<Dictionary<string,string>> GetWeather(string query)
        {
            try
            {
                Init();
                query = query.Replace(" ", "%20").Replace(",", "%2C");
                var requestUri = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{query}?unitGroup=us&key={_apiKey}&contentType=json";
                var response = _httpClient.GetAsync(requestUri).Result;
                if (response.IsSuccessStatusCode)
                {
                    HttpContent content = response.Content;
                    string result = await content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<WeatherList>(result);
                    string tempMax = list.days.First<Weather>().tempmax.ToString();
                    string tempMin = list.days.First<Weather>().tempmin.ToString();
                    string icon = list.days.First<Weather>().icon;
                    return new Dictionary<string, string>
                    {
                        {"StatusCode",response.StatusCode.ToString() },
                        { "tempmax",tempMax },
                        {"tempmin",tempMin },
                        {"icon",icon }

                    };
                }
                
                return new Dictionary<string, string>()
                {
                    {"StatusCode",response.StatusCode.ToString() }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                return new Dictionary<string, string>(){
                    {"StatusCode","NOT_FOUND" }
                };
            }
            
        }
    }
}
