namespace MinimalApiToDoAPI.Entities;

public partial class Game
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Publisher { get; set; }

    public DateTime? ReleaseDate { get; set; }
}
