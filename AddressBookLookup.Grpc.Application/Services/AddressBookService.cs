using AddressBookLookup.Grpc.Domain.Interfaces;
using AddressBookLookup.Grpc.Application.Interfaces;
using Google.Protobuf.WellKnownTypes;
using AddressBookLookup.Grpc.Application.Mapper;

namespace AddressBookLookup.Grpc.Application.Services;

/// <summary>
/// Service layer implementation for address book operations.
/// </summary>
public class AddressBookService(IAddressBookRepository repository) : IAddressBookService
{
    /// <summary>
    /// Retrieves all people from the address book.
    /// </summary>
    /// <returns>A collection of people in the address book.</returns>
    public async Task<IEnumerable<Common.Person>> GetAddressBookAsync()
    {
        var people = await repository.GetAllAsync();
        return people.Select(p => p.ToProtoEntity());
    }

    /// <summary>
    /// Searches for people matching the given criteria, respecting the field mask.
    /// The field mask determines which fields should be used for comparison.
    /// </summary>
    /// <param name="filter">The person to use as a filter for searching.</param>
    /// <param name="fieldMask">Specifies which fields should be used for comparison.</param>
    /// <returns>A collection of matching people.</returns>
    public async Task<IEnumerable<Common.Person>> FindPersonAsync(Common.Person filter, FieldMask fieldMask)
    {
        var maskedPerson = ApplyFieldMask(filter, fieldMask);
        var filteredPeople = await repository.FindPersonByFilterAsync(maskedPerson);
        return filteredPeople.Select(p => p.ToProtoEntity());
    }

    /// <summary>
    /// Applies a field mask to a person, creating a new person with only the specified fields.
    /// This ensures that only the fields specified in the mask are used for comparison.
    /// </summary>
    /// <param name="person">The person to apply the mask to.</param>
    /// <param name="fieldMask">The field mask specifying which fields to include.</param>
    /// <returns>A new person with only the masked fields populated.</returns>
    private static Domain.Entities.Person ApplyFieldMask(Common.Person person, FieldMask fieldMask)
    {
        var maskedPerson = new Common.Person();

        foreach (var path in fieldMask.Paths)
        {
            switch (path)
            {
                case var p when p.Equals(nameof(Common.Person.Name), StringComparison.CurrentCultureIgnoreCase):
                    maskedPerson.Name = person.Name;
                    break;
                case var p when p.Equals(nameof(Common.Person.Email), StringComparison.CurrentCultureIgnoreCase):
                    maskedPerson.Email = person.Email;
                    break;
                case var p when p.Equals(nameof(Common.Person.Address), StringComparison.CurrentCultureIgnoreCase):
                    maskedPerson.Address = person.Address;
                    break;
            }
        }

        return maskedPerson.ToDomainEntity();
    }
}