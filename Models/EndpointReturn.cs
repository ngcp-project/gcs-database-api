using System.Text.Json;

namespace Database.Models
{
    public class EndpointReturn
    {
        public string message { get; set; }
        public string error { get; set; }
        public string data { get; set; }

        public EndpointReturn(string message, string error, string data)
        {
            this.message = message;
            this.error = error;
            this.data = data;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}