using AddressBookLookup.Grpc.Common;
using Google.Protobuf.WellKnownTypes;

namespace AddressBookLookup.Grpc.Application.Interfaces
{
    public interface IAddressBookService
    {
        Task<IEnumerable<Person>> GetAddressBookAsync();
        Task<IEnumerable<Person>> FindPersonAsync(Person filter, FieldMask fieldMask);
    }
}
