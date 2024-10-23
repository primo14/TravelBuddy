using Android.Gms.Common.Api.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TripPlanner.Model;


namespace TripPlanner.Services
{
    public class GooglePlaceServices
    {

        private static HttpClient _httpClient = new HttpClient();
        private static string _apiKey = "";                             //Insert Keys.txt file in Resouces/raw folder and put your own API key in 1st line of the file

        public GooglePlaceServices()
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
                _apiKey = reader.ReadToEndAsync().Result.Split("\n")[0];
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }

        public static async Task<List<Prediction>> GetPlacesPredictionAsync(string search)
        {
            Init();
            var requestUri = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={search}&key={_apiKey}";
            var response = _httpClient.GetAsync(requestUri).Result;
            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;
                string result = await content.ReadAsStringAsync();
               var list = JsonConvert.DeserializeObject<PlacesResponse>(result);

                return list.Predictions;
            }
            return new List<Prediction>();
        }

        public static async Task<string> GetLatLongAsync(string place_id)
        {
            Init();
            var requestUri = $"https://maps.googleapis.com/maps/api/place/details/json?fields=geometry&placeid={place_id}&key={_apiKey}";
            var response = _httpClient.GetAsync(requestUri).Result;
            if (response.IsSuccessStatusCode)
            {
                HttpContent content = response.Content;
                string result = await content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<PlaceDetails>(result);

                return details?.result.geometry.location.lat +',' + details?.result.geometry.location.lng;
            }
            return "";
        }
    }
}
