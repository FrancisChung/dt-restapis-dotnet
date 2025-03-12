﻿using Microsoft.AspNetCore.Mvc;
using Movies.API.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;
using Movies.Application.Models;
using Movies.Contracts.Responses;

namespace Movies.API.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
        {
            var movie = request.MapToMovie(); 
            var result = await _movieRepository.CreateAsync(movie); 
            return CreatedAtAction(nameof(Get),new {idOrSlug = movie.Id}, movie );
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get([FromRoute] string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieRepository.GetByIdAsync(id)
                : await _movieRepository.GetBySlugAsync(idOrSlug);

            if (movie is null)
                return NotFound();

            return Ok(movie.MapToResponse());
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _movieRepository.GetAllAsync();
            if (movies is null)
                return NotFound();

            return Ok(movies.MapToResponse());
        }

        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
        {
            var movie = request.MapToMovie(id);
            var updated = await _movieRepository.UpdateAsync(movie);
            if (updated)
                return Ok(movie.MapToResponse());
            else
                return NotFound();
        }

        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var updated = await _movieRepository.DeleteByIdAsync(id);
            if (updated)
                return Ok();
            else
                return NotFound();
        }
    }
}
