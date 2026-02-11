namespace MinimalApiToDoAPI.Models
{
    public class GameDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime ReleaseDate {  get; set; } = DateTime.Now;
    }
}
