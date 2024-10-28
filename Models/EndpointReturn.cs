using System.Text.Json;

namespace Database.Models
{
    public class EndpointReturn<T>
        {
        public string message { get; set; }
        public string error { get; set; }

        //String -> Generic
        public T data { get; set; } 

        public EndpointReturn(string message, string error, T data)
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