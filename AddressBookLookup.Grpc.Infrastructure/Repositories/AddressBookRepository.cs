using AddressBookLookup.Grpc.Domain.Entities;
using AddressBookLookup.Grpc.Domain.Interfaces;
using AddressBookLookup.Grpc.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AddressBookLookup.Grpc.Infrastructure.Repositories
{
    public class AddressBookRepository(AppDbContext context) : IAddressBookRepository
    {
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await context.People.ToListAsync();
        }   

        public async Task<IEnumerable<Person>> FindPersonByFilterAsync(Person filter)
        {
            IQueryable<Person> query = context.People;

            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Address))
                query = query.Where(p => p.Address.Contains(filter.Address));

            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(p => p.Email.Contains(filter.Email));

            return await query.ToListAsync();
        }
    }
}
