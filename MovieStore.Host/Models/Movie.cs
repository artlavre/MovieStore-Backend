namespace MovieStore.Host.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string? CoverUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public List<string> Actors { get; set; } = new List<string>();
    public float Rating { get; set; }
    public DateTime ReleaseDate { get; set; }
}