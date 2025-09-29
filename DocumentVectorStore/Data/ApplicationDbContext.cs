using Microsoft.EntityFrameworkCore;
using DocumentVectorStore.Models;

namespace DocumentVectorStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DocumentStore> DocumentStores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DocumentStore>(entity =>
            {
                entity.HasIndex(e => e.VectorStoreId).IsUnique();
                entity.HasIndex(e => e.FileId).IsUnique();
            });
        }
    }
}