using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using MySqlApi.Models;

namespace MySqlApi.DataBase
{
    public class KobaContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        public KobaContext(DbContextOptions<KobaContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Log>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}