using Microsoft.AspNetCore.Mvc;
using MovieStore.Host.Contracts;
using MovieStore.Host.DTOs;

namespace MovieStore.Host.Controllers;

[ApiController]
[Route("api/")]
public class MovieController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;
    private readonly IMinioService _minioService;
    
    public MovieController(IMovieRepository movieRepository, IMinioService minioService)
    {
        _movieRepository = movieRepository;
        _minioService = minioService;
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

    [HttpPost("MinioCover")]
    public async Task<ActionResult> AddCover(IFormFile file, string fileName, string contentType)
    {
        await _minioService.UploadAsync(file, fileName);
        return Ok();
    }
}