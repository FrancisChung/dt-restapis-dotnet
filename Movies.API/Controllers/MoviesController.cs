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

           var response = new MovieResponse
           {
               Id = movie.Id,
               Title = movie.Title,
               Genres = movie.Genres,
               YearOfRelease = movie.YearOfRelease
           };
           //return Ok(response);
           return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
        }
    }
}
