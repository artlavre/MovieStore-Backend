namespace MovieStore.Host.Models;

public class MinioConfiguration
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PhotosBucket { get; set; } = string.Empty;
}