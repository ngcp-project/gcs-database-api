namespace Database.Controllers
{
    public class VehicleDataController
    {
        static void Main(string[] args)
        {
            using(var client = new HttpClient())
            {
                var endpoint = new Uri("https://jsonplaceholder.typicode.com/posts");
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}