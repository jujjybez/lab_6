using System;
using System.Text;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json.Linq;

class Program
{
    struct Weather
    {
        public string Country { get; set; }
        public string Name { get; set; }
        public double Temp { get; set; }
        public string Description { get; set; }
    }
    public static void Main(string[] args)
    {
        Weather[] array = new Weather[50];
        string api_key = "5cc53f61b2c0f1891c6bbb11b670a9f4";
        string URL = "https://api.openweathermap.org/data/2.5/weather";

        Console.WriteLine();

        for (int i = 0; i < 50; i++)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("aalocation/json"));
            Random rand = new Random();
            int lat = (int)(rand.NextDouble() * (90 - (-90)) + (-90));
            int lon = (int)(rand.NextDouble() * (180 - (-180)) + (-180));
            string urlParameters = $"?lat={lat}.&lon={lon}.&appid={api_key}";
            Console.WriteLine(URL + urlParameters);
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;

            if (response.IsSuccessStatusCode)
            {
                var data_obj = response.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(data_obj);
                Weather res = new Weather();
                res.Country = (string)json["sys"]["country"];
                res.Name = (string)json["name"];
                res.Temp = (double)json["main"]["temp"];
                res.Description = (string)json["weather"][0]["main"];
                array[i] = res;
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }
            client.Dispose();
        }
        Console.WriteLine("Data is received.");
        Console.WriteLine("Minimal temperature: ");
        Weather sCTM = array.OrderBy(w => w.Temp).First();
        Console.WriteLine($"{sCTM.Country} {sCTM.Name} {sCTM.Temp} {sCTM.Description}");

        Console.WriteLine("Maximal temperature: ");
        Weather sCTMax = array.OrderByDescending(w => w.Temp).First();
        Console.WriteLine($"{sCTMax.Country}  {sCTMax.Name}  {sCTMax.Temp}  {sCTMax.Description}");

        Console.WriteLine("Average temperature: ");
        double average = array.Average(r => r.Temp);
        Console.WriteLine(average);

        Console.WriteLine("Amount of countries: ");
        int number = array.GroupBy(r => r.Country).Count();
        Console.WriteLine(number);

        var re = (from w in array where (w.Description == "Clear sky" || w.Description == "Rain" || w.Description == "Clouds") select w);
        if (re.Any())
        {
            Weather rest = re.First();
            Console.WriteLine("Clear sky, rain or few clouds: ");
            Console.WriteLine($"{rest.Country}  {rest.Name}  {rest.Temp}  {rest.Description}");
        }
        else
        {
            Console.WriteLine("Needed description is not found.");
        }
    }
}
