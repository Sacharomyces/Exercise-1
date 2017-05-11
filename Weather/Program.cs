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
    
    public class City
    {
        public string Name { get; set; }
        public int Temp { get; set; }

    }
    class Program
    {
        static  HttpClient client = new HttpClient();


        static List<string> GetIds()
        {
            Random random = new Random();

            var fullArray = JArray.Parse(File.ReadAllText(@"city.list.json"));  
            var ids = new List<string>();                                       // this could be hardcoded list of 50 strings, but I want to achive different results each time

                for (int i = 0; i < 50; i++)
                {
                    var item = fullArray[random.Next(fullArray.Count)];
                    var id = (string) item["id"];
                    ids.Add(id);
                    fullArray.Remove(item);
                }
            return ids;
        }

        static List<City> GetCities()
        {
            var baseUrl = "http://api.openweathermap.org/data/2.5/weather?id=";
            var paramUrl = "&&APPID=cafbd44dc972394a979fd67edeb77556&&units=metric";
            var cityList = new List<City>();

            var ids = GetIds();


            foreach (string id in ids)
            {

                var city = GetCityAsync(baseUrl + id + paramUrl).Result;

                cityList.Add(city);

            }

            return cityList;

        }

        static void HighestTemps()
        {
            var cityList = GetCities();
            cityList.Sort((city1, city) => city.Temp.CompareTo(city1.Temp));

            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(cityList[i].Name + " - Temp:" + cityList[i].Temp +"°C");
            }
        }

        static async Task<City> GetCityAsync(string path) 
        {
            var city = new City();
            

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var jobj = await response.Content.ReadAsAsync<JObject>();
                city.Name = (string) jobj["name"];
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
