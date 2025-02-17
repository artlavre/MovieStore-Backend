using Marten;
using MovieStore.Host.Contracts;
using MovieStore.Host.DTOs;
using MovieStore.Host.Models;

namespace MovieStore.Host.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDocumentStore _documentStore;
    private readonly IMinioService _minioService;
    
    public MovieRepository(IDocumentStore documentStore, IMinioService minioService)
    {
        _documentStore = documentStore; 
        _minioService = minioService;
    }

    public async Task<Movie?> GetMovieById(Guid id)
    {
        using (var session = _documentStore.LightweightSession())
        {
            var movie = await session.LoadAsync<Movie>(id);
            
            if (movie == null)
            {
                return null;
            }
        
            return movie;
        }
    }

    public async Task<IReadOnlyList<Movie>?> GetMovies(MovieQueryDto movieQueryDto)
    {
        using (var session = _documentStore.LightweightSession())
        {
            IQueryable<Movie> movies = session.Query<Movie>();
            
            if (!string.IsNullOrWhiteSpace(movieQueryDto.Title))
            {
                movies = movies.Where(m => m.Title.ToLower().Contains(movieQueryDto.Title.ToLower()));
            }

            var sortedMovies = SortMovies(movies, movieQueryDto);
            
            return await sortedMovies.ToListAsync();;   
        }
    }

    public async Task<Movie?> AddMovie(AddMovieDto addMovieDto)
    {
        using (var session = _documentStore.LightweightSession())
        {
            if (string.IsNullOrEmpty(addMovieDto.Title) || string.IsNullOrEmpty(addMovieDto.Description))
            {
                return null;
            }

            var newMovie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = addMovieDto.Title,
                Description = addMovieDto.Description,
                Language = addMovieDto.Language,
                Actors = addMovieDto.Actors,
                Rating = addMovieDto.Rating,
                ReleaseDate = addMovieDto.ReleaseDate
            };
            
            if (addMovieDto.MovieCover != null)
            {
                await SetCoverUrl(newMovie, addMovieDto.MovieCover);
            }
        
            session.Store(newMovie);
            await session.SaveChangesAsync();
        
            return newMovie;
        }

    }

    private IQueryable<Movie> SortMovies(IQueryable<Movie> movies, MovieQueryDto movieQueryDto)
    {
        if (!string.IsNullOrEmpty(movieQueryDto.SortBy))
        {
            switch (movieQueryDto.SortBy.ToLower())
            {
                case "title":
                    movies = movieQueryDto.Descending ? movies.OrderByDescending(m => m.Title) : movies.OrderBy(m => m.Title);
                    break;
                case "releasedate":
                    movies = movieQueryDto.Descending ? movies.OrderByDescending(m => m.ReleaseDate) : movies.OrderBy(m => m.ReleaseDate);
                    break;
                default:
                    movies = movies.OrderBy(m => m.Title);
                    break;
            }
        }

        return movies;
    }

    private async Task SetCoverUrl(Movie movie, IFormFile coverImage)
    {
        await _minioService.UploadAsync(coverImage, movie.Id.ToString());
        
        var fileUrl = await _minioService.GetAsync(movie.Id.ToString());
        
        movie.CoverUrl = fileUrl;
    }
}