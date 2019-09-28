using CW03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CW03.Data
{
    public class CW03Context : DbContext
    {
        public CW03Context(DbContextOptions<CW03Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemLocation>().Ignore(l => l.Latitude);
            modelBuilder.Entity<ItemLocation>().Ignore(l => l.Longitude);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<BookmarkEntity> BookmarkEntity { get; set; }

        public DbSet<Folder> Folder { get; set; }

        public DbSet<Item> Item { get; set; }

        public DbSet<ItemLink> ItemLink { get; set; }

        public DbSet<ItemLocation> ItemLocation { get; set; }

        public DbSet<ItemTextFile> ItemTextFile { get; set; }
    }

    public class CW03ContextFactory : IDesignTimeDbContextFactory<CW03Context>
    {

        public CW03Context CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
            var optionsBuilder = new DbContextOptionsBuilder<CW03Context>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("CW03Context"));
            return new CW03Context(optionsBuilder.Options);
        }
    }
}
