using System;
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
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS movies (
                    id UUID PRIMARY KEY,
                    slug TEXT not null,
                    title TEXT NOT NULL,
                    yearofrelease INTEGER NOT NULL,
                    genre TEXT NOT NULL
                );
            ");

            await connection.ExecuteAsync(@"
                CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS idx_movies_slug
                ON movies using btree(slug);
            ");
        }
    }
}
