using Microsoft.AspNetCore.Mvc;
using Movies.API.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Application.Services;
using Movies.Contracts.Responses;

namespace Movies.API.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
        {
            var movie = request.MapToMovie(); 
            var result = await _movieService.CreateAsync(movie, token); 
            return CreatedAtAction(nameof(Get),new {idOrSlug = movie.Id}, movie );
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
        {
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, token)
                : await _movieService.GetBySlugAsync(idOrSlug, token);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll(CancellationToken token)
        {
            var movies = await _movieService.GetAllAsync(token);
            if (movies is null)
                return NotFound();

            return Ok(movies.MapToResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request, CancellationToken token)
        {
            var movie = request.MapToMovie(id);
            var updated = await _movieService.UpdateAsync(movie, token);
            if (updated is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
        {
            var updated = await _movieService.DeleteByIdAsync(id, token);
            if (updated)
                return Ok();
            else
                return NotFound();
        }
    }
}
