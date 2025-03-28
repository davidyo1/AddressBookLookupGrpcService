using AddressBookLookup.Grpc.Domain.Interfaces;
using AddressBookLookup.Grpc.Application.Interfaces;
using Google.Protobuf.WellKnownTypes;
using AddressBookLookup.Grpc.Application.Mapper;

namespace AddressBookLookup.Grpc.Application.Services;

public class AddressBookService(IAddressBookRepository repository) : IAddressBookService
{
    public async Task<IEnumerable<Common.Person>> GetAddressBookAsync()
    {
        var people = await repository.GetAllAsync();
        return people.Select(p => p.ToProtoEntity());
    }

    public async Task<IEnumerable<Common.Person>> FindPersonAsync(Common.Person filter, FieldMask fieldMask)
    {
        var maskedPerson = ApplyFieldMask(filter, fieldMask);
        var filteredPeople = await repository.FindPersonByFilterAsync(maskedPerson);
        return filteredPeople.Select(p => p.ToProtoEntity());
    }

    private static Domain.Entities.Person ApplyFieldMask(Common.Person person, FieldMask fieldMask)
    {
        var maskedPerson = new Common.Person();

        foreach (var path in fieldMask.Paths)
        {
            if (path == nameof(Common.Person.Name)) maskedPerson.Name = person.Name;
            else if (path == nameof(Common.Person.Email)) maskedPerson.Email = person.Email;
            else if (path == nameof(Common.Person.Address)) maskedPerson.Address = person.Address;
        }

        return maskedPerson.ToDomainEntity();
    }
}