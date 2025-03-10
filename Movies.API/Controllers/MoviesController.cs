using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Contracts.Responses;

namespace Movies.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpPost("movies")]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
        {
            var movie = new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Genres = request.Genres.ToList(),
                YearOfRelease = request.YearOfRelease
            };
           var result = await _movieRepository.CreateAsync(movie);

           var response = new MovieResponse
           {
               Id = movie.Id,
               Title = movie.Title,
               Genres = movie.Genres,
               YearOfRelease = movie.YearOfRelease
           };
           //return Ok(response);
           return Created($"/api/movies/{movie.Id}", movie);
        }
    }
}
