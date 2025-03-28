using AddressBookLookup.Grpc.Domain.Entities;

namespace AddressBookLookup.Grpc.Domain.Interfaces
{
    public interface IAddressBookRepository
    {
        Task<IEnumerable<Person>> FindPersonByFilterAsync(Person filter);
        Task<IEnumerable<Person>> GetAllAsync();
    }
}