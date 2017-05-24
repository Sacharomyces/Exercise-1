using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Weather
{
    
    
    class Program
    {
        static  HttpClient client = new HttpClient();


        static List<City> GetCities()
        {
            Random random = new Random();

            var allCities = new List<City>();

            var cities = new List<City>();

            using (StreamReader sr = File.OpenText(@"city.list.json"))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();

                    allCities = serializer.Deserialize<List<City>>(reader);
                }
            }
          

            for (int i = 0; i < 50; i++)
            {
                    var city = allCities[random.Next(allCities.Count)];
                    cities.Add(city);
                    allCities.Remove(city);
            }

            return cities;
        }


       static List<City> GetTemps()
        {
            var baseUrl = "http://api.openweathermap.org/data/2.5/weather?id=";
            var paramUrl = "&&APPID=cafbd44dc972394a979fd67edeb77556&&units=metric";
            var cities = GetCities();


            foreach (City city in cities)
            {

                city.Temp = GetCityAsync(baseUrl + city.Id + paramUrl).Result.Temp;
                
            }

            return cities;

        }

        static void HighestTemps()
        {
            var citiesWithTemps = GetTemps();
            citiesWithTemps.Sort((city1, city) => city.Temp.CompareTo(city1.Temp));

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(citiesWithTemps[i].Name + " - Temp:" + citiesWithTemps[i].Temp +"°C");
            }
        }

        static async Task<City> GetCityAsync(string path) 
        {
            var city = new City();
            

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var jobj = await response.Content.ReadAsAsync<JObject>();
                city.Temp = (int) jobj["main"]["temp"];

            }

            return city ;
        }



        static void Main(string[] args)
        {
          Run();
        }

        static void Run()
        {
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HighestTemps();
            
            
            Console.ReadLine();
        }


    }
}
