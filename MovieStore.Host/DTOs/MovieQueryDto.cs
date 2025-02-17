namespace MovieStore.Host.DTOs;

public class MovieQueryDto
{
    public string? Title { get; set; } = string.Empty;
    public string SortBy { get; set; } = "title";
    public bool Descending { get; set; } = false;
}