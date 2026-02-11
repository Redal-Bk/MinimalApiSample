namespace MinimalApiToDoAPI.Models
{
    public class ResponseModel
    {
        public string message { get; set; } = string.Empty;
        public bool success { get; set; }
    }
    public class ResponseModel<T>
    {
        public string message { get; set; } = string.Empty;
        public bool success { get; set; }
        public T response { get; set; }
    }
}
