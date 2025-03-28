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
                new Person { Id = 1, Name = "Nombre1 Prueba1", Address = "Calle Falsa, 123", Email = "nombre1@test.com" },
                new Person { Id = 2, Name = "Nombre2 Prueba2", Address = "Calle Falsa, 456", Email = "nombre2@test.com" },
                new Person { Id = 3, Name = "Nombre3 Prueba3", Address = "Calle Falsa, 456", Email = "nombre2@test.com" }
            );
        }
    }
}
