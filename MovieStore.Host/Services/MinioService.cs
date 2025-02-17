using Minio;
using Minio.DataModel.Args;
using MovieStore.Host.Contracts;
using MovieStore.Host.Models;

namespace MovieStore.Host.Services;

public class MinioService : IMinioService
{
    private readonly IMinioClient _client;
    private readonly MinioConfiguration _minioConfig;
    
    public MinioService(IMinioClient minioClient, IConfiguration configuration)
    {
        _client = minioClient;
        _minioConfig = configuration.GetSection("Minio").Get<MinioConfiguration>()!;
    }

    public async Task UploadAsync(IFormFile file, string fileName)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_minioConfig.PhotosBucket);
        bool bucketFound = await _client.BucketExistsAsync(bucketExistsArgs);

        if (!bucketFound)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_minioConfig.PhotosBucket);
            await _client.MakeBucketAsync(makeBucketArgs);
        }
        
        var inputStream = file.OpenReadStream();
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_minioConfig.PhotosBucket).WithObject(fileName)
            .WithStreamData(inputStream)
            .WithObjectSize(inputStream.Length)
            .WithContentType("image/jpeg");
        
        await _client.PutObjectAsync(putObjectArgs);
    }

    public async Task<string?> GetAsync(string fileName)
    {
        if (!await ObjectExistsAsync(fileName))
        {
            return null;
        }
        
        var presignedObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(_minioConfig.PhotosBucket)
            .WithObject(fileName)
            .WithExpiry(24 * 60 * 60);
        
        return await _client.PresignedGetObjectAsync(presignedObjectArgs);
    }
    
    private async Task<bool> ObjectExistsAsync(string fileName)
    {
        try
        {
            await _client.StatObjectAsync(new StatObjectArgs()
                .WithBucket(_minioConfig.PhotosBucket)
                .WithObject(fileName));

            return true;
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            Console.WriteLine("Object not found...");
            
            return false;
        }
    }
}