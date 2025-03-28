﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Movies.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DbInitializer(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InitializeAsync()
        {
            /*using var connection2 = await _dbConnectionFactory.CreateConnectionAsync();
            await connection2.ExecuteAsync(@"
                DROP TABLE genres;
                DROP TABLE movies;
            ")*/;

            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS movies (
                    id UUID PRIMARY KEY,
                    slug TEXT not null,
                    title TEXT NOT NULL,
                    yearofrelease INTEGER NOT NULL
                );
            ");

            await connection.ExecuteAsync(@"
                CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS movies_slug_idx
                ON movies using btree(slug);
            ");

            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS genres (
                    movieId UUID references movies(Id),
                    name TEXT NOT NULL);
            ");
        }
    }
}
