using CW03.Models;
using Microsoft.EntityFrameworkCore;

namespace CW03.Data
{
    public class CW03Context : DbContext
    {
        public CW03Context (DbContextOptions<CW03Context> options)
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
}
