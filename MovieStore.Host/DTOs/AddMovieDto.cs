namespace MovieStore.Host.DTOs;

public class AddMovieDto
{
    public IFormFile? MovieCover { get; set; } = null;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "En";
    public float Rating { get; set; } = 8.0f;
    public List<string> Actors { get; set; } = new List<string>();
    public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
}