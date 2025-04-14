using Marten;
using MovieStore.Host.Contracts;
using MovieStore.Host.Models;

namespace MovieStore.Host.HostedServices;

public class CoverUpdateService : BackgroundService
{
    private readonly IDocumentStore _documentStore;
    private readonly IMinioService _minioService;
    
    public CoverUpdateService(IDocumentStore documentStore, IMinioService minioService)
    {
        _documentStore = documentStore;
        _minioService = minioService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var session = _documentStore.LightweightSession();
            
            var thresholdDate = DateTime.Now.AddHours(12);
            
            var movies = await session.Query<Movie>().Where(m => m.CoverExpiryDate < thresholdDate).ToListAsync(stoppingToken);
            var updatableMovies = movies.ToList();

            foreach (var movie in updatableMovies)
            {
                await UpdateAndSaveCoverPath(session, movie); 
            }
            
            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }

    private async Task UpdateAndSaveCoverPath(IDocumentSession session, Movie movie)
    {
        var fileUrl = await _minioService.GetCoverAsync(movie.Id.ToString());
        
        movie.CoverUrl = fileUrl;
        movie.CoverExpiryDate = DateTime.UtcNow.AddDays(7);
        
        session.Store(movie);
        await session.SaveChangesAsync();
    }
}