using AddressBookLookup.Grpc.Domain.Entities;
using AddressBookLookup.Grpc.Domain.Interfaces;
using AddressBookLookup.Grpc.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AddressBookLookup.Grpc.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for address book persistence operations.
    /// </summary>
    public class AddressBookRepository(AppDbContext context) : IAddressBookRepository
    {
        /// <summary>
        /// Retrieves all people from the database.
        /// </summary>
        /// <returns>A collection of all people in the address book.</returns>
        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await context.People.ToListAsync();
        }   

        /// <summary>
        /// Searches for people matching the given filter criteria.
        /// </summary>
        /// <param name="filter">The person to use as a filter for searching.</param>
        /// <returns>A collection of people matching the filter criteria.</returns>
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
