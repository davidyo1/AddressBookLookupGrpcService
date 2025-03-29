using Microsoft.EntityFrameworkCore;
using AddressBookLookup.Grpc.Domain.Entities;

namespace AddressBookLookup.Grpc.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, Name = "Name1 Test1", Address = "Fake Street, 123", Email = "name1@test.com" },
                new Person { Id = 2, Name = "Name2 Test2", Address = "Fake Street, 456", Email = "name2@test.com" },
                new Person { Id = 3, Name = "Name3 Test3", Address = "Fake Street, 456", Email = "name2@test.com" }
            );
        }
    }
}
