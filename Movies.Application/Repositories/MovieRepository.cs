using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories
{
    internal class MovieRepository : IMovieRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }
        public async Task<bool> CreateAsync(Movie movie)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            using var transaction = connection.BeginTransaction();

            var result = await connection.ExecuteAsync(new CommandDefinition(@"
                INSERT INTO movies (id, slug, title, yearofrelease)
                VALUES (@Id, @Slug, @Title, @YearOfRelease)
                ", movie));
            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition(@"
                        INSERT INTO genres (movieId, name)
                        VALUES (@MovieId, @Name)
                    ", new { MovieId = movie.Id, Name = genre }));
                }
            }
            transaction.Commit();
            return result > 0;
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
                new CommandDefinition(@"
                SELECT id, slug, title, yearofrelease
                FROM movies
                WHERE id = @Id", new { id }));
            
            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(@"
                SELECT name
                FROM genres
                WHERE movieId = @Id", new { id }));
            
            foreach (var genre in genres)
                movie.Genres.Add(genre);

            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
                new CommandDefinition(@"
                SELECT id, slug, title, yearofrelease
                FROM movies
                WHERE slug = @slug", new { slug }));

            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(@"
                SELECT name
                FROM genres
                WHERE movieId = @Id", new { id = movie.Id }));

            foreach (var genre in genres)
                movie.Genres.Add(genre);

            return movie;
        }

        public Task<IEnumerable<Movie>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByIdAsync(Guid id)
        {
            throw new NotImplementedException(); ;
        }
    }
}
