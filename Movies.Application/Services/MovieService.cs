﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IValidator<Movie> _movieValidator;

        public MovieService(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
        {
            _movieRepository = movieRepository;
            _movieValidator = movieValidator;
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);
            return await _movieRepository.CreateAsync(movie, token);
        }
        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            return _movieRepository.DeleteByIdAsync(id, token);
        }
        public Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            return _movieRepository.ExistsByIdAsync(id, token);
        }
        public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            return _movieRepository.GetAllAsync(token);
        }
        public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            return _movieRepository.GetBySlugAsync(slug, token);
        }
        public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return _movieRepository.GetByIdAsync(id, token);
        }
        public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            await _movieValidator.ValidateAndThrowAsync(movie, token);
            var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id);

            if (!movieExists)
            {
                return null;
            }

            await _movieRepository.UpdateAsync(movie, token) ;
            return movie;
        }
    }
}
