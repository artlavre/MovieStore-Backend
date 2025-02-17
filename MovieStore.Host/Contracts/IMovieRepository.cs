using MovieStore.Host.DTOs;
using MovieStore.Host.Models;

namespace MovieStore.Host.Contracts;

public interface IMovieRepository
{
    public Task<Movie?> GetMovieById(Guid id);
    public Task<IReadOnlyList<Movie>?> GetMovies(MovieQueryDto movieQueryDto);
    public Task<Movie?> AddMovie(AddMovieDto addMovieDto);
}