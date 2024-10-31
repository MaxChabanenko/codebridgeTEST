using codebridgeTEST.Models;
using Microsoft.EntityFrameworkCore;

namespace codebridgeTEST.Data
    {
        public class DataContext : DbContext
        {
            public DataContext(DbContextOptions<DataContext> options) : base(options)
            {

            }

            public DbSet<Dog> Dogs { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Dog>().HasData(
                    new Dog
                    {
                        Name = "Neo",
                        TailLength = 22,
                        Color = "red&amber",
                        Weight = 32
                    },
                    new Dog
                    {
                        Name = "Jessy",
                        TailLength = 7,
                        Color = "black&white",
                        Weight = 14
                    }
                );
            }
        }
}
