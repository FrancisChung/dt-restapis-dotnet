using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public Task<bool> CreateAsync(Movie movie)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<bool> ExistsByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Movie>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<Movie?> GetBySlugAsync(string slug)
        {
            throw new NotImplementedException();
        }
        public Task<Movie?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<Movie?> UpdateAsync(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
