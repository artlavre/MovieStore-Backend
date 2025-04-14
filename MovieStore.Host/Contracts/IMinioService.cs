namespace MovieStore.Host.Contracts;

public interface IMinioService
{
    Task UploadAsync(IFormFile file, string fileName);
    Task<string?> GetCoverAsync(string fileName);
}