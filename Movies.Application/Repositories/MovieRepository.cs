﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
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
                    ", new { MovieId = movie.Id, Name = genre }, cancellationToken:token));
                }
            }
            transaction.Commit();
            return result > 0;
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
                new CommandDefinition(@"
                SELECT id, slug, title, yearofrelease
                FROM movies
                WHERE id = @Id", new { id }, cancellationToken: token));
            
            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(@"
                SELECT name
                FROM genres
                WHERE movieId = @Id", new { id }, cancellationToken: token));
            
            foreach (var genre in genres)
                movie.Genres.Add(genre);

            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
                new CommandDefinition(@"
                SELECT id, slug, title, yearofrelease
                FROM movies
                WHERE slug = @slug", new { slug }, cancellationToken: token));

            if (movie is null)
                return null;

            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(@"
                SELECT name
                FROM genres
                WHERE movieId = @Id", new { id = movie.Id }, cancellationToken: token));

            foreach (var genre in genres)
                movie.Genres.Add(genre);

            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            var result = await connection.QueryAsync(new CommandDefinition(@"
                SELECT id, slug, title, yearofrelease, string_agg(g.name,',') as genres
                FROM movies m left join genres g on m.id = g.movieid group by id", cancellationToken: token));

            var movies = result.Select(x => new Movie
            {
                Id = x.id,
                Title = x.title,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(','))
            });
            return movies;
        }

        public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();
            connection.ExecuteAsync(new CommandDefinition(@"
                Delete from genres where movieid=@id", new { id = movie.Id }, cancellationToken: token));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(@"
                        INSERT INTO genres (movieId, name)
                        VALUES (@MovieId, @Name)
                    ", new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }

            var result = await connection.ExecuteAsync(new CommandDefinition(@"
                UPDATE movies
                SET slug = @Slug, title = @Title, yearofrelease = @YearOfRelease
                WHERE id = @Id", movie));

            transaction.Commit();
            return result > 0;
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            using var transaction = connection.BeginTransaction();
            await connection.ExecuteAsync(new CommandDefinition(@"
                Delete from genres where movieid=@id", new { id }));

            var result = await connection.ExecuteAsync(new CommandDefinition(@"
                Delete from movies where id=@id", new { id  }));

            transaction.Commit();
            return result > 0;
        }

        public async  Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(token);
            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(@"
                SELECT EXISTS(SELECT 1 FROM movies WHERE id = @Id)", new { id }));
        }
    }
}
