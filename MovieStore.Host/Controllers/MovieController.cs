using Microsoft.AspNetCore.Mvc;
using MovieStore.Host.Contracts;
using MovieStore.Host.DTOs;

namespace MovieStore.Host.Controllers;

[ApiController]
[Route("api/")]
public class MovieController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;
    
    public MovieController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpGet("movies/{id}")]
    public async Task<ActionResult> GetMovieByName(Guid id)
    {
        var result = await _movieRepository.GetMovieById(id);
        
        return Ok(result);
    }
    
    [HttpGet("movies")]
    public async Task<ActionResult> GetMovies([FromQuery] MovieQueryDto query)
    {
        var result = await  _movieRepository.GetMovies(query);
        
        return Ok(result);
    }

    [HttpPost("movies")]
    public async Task<ActionResult> AddMovie([FromForm] AddMovieDto addMovieDto)
    {
        var result = await _movieRepository.AddMovie(addMovieDto);
        
        return Ok(result);
    }
}