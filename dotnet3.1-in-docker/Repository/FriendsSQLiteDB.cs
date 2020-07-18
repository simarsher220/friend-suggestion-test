using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet3._1_in_docker.Repository
{
    public class FriendSuggestorContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<FriendsList> FriendsLists { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite("Data Source=FriendSuggester.db");
        public FriendSuggestorContext()
        {
        }

        public FriendSuggestorContext(DbContextOptions<FriendSuggestorContext> options) : base(options)
        {
        }
    }

    public class FriendSuggestorContextFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<FriendSuggestorContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<FriendSuggestorContext>()
                .UseSqlite(_connection).Options;
        }

        public FriendSuggestorContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=FriendSuggestor.db");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new FriendSuggestorContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new FriendSuggestorContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
