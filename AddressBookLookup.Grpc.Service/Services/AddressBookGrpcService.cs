using AddressBookLookup.Grpc.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Linq;

namespace AddressBookLookup.Grpc.Server.Services
{
    public class AddressBookGrpcService : AddressBookLookupService.AddressBookLookupServiceBase
    {
        private readonly ILogger<AddressBookGrpcService> _logger;
        private readonly AddressBook _addressBook;
        public AddressBookGrpcService(ILogger<AddressBookGrpcService> logger)
        {
            _logger = logger;
            //temp
            _addressBook = new AddressBook
            {
                People =
                {
                    new Person { Name = "Name1 Test1", Address="Fake street, 123", Email = "name1@test.com" },
                    new Person { Name = "Name2 Test2", Address="Fake street, 456", Email = "name2@test.com" }
                }
            };
        }

        public override Task<AddressBook> GetAddressBook(Empty request, ServerCallContext context)
        {
            _logger.LogDebug("GetAddressBook called");
            return Task.FromResult(_addressBook);
        }

        public override async Task FindPerson(FindPersonRequest request, IServerStreamWriter<Person> responseStream, ServerCallContext context)
        {
            _logger.LogDebug($"FindPerson called with filters: {request}");
            foreach (var person in _addressBook.People.Where(person => MatchesFilter(person, request.Person, request.FieldMask)))
            {
                await responseStream.WriteAsync(person);
            }
        }

        private static bool MatchesFilter(Person storedPerson, Person filterPerson, FieldMask fieldMask)
        {
            foreach (var field in fieldMask.Paths)
            {
                switch (field)
                {
                    case "name":
                        if (storedPerson.Name != filterPerson.Name) return false;
                        break;
                    case "address":
                        if (storedPerson.Address != filterPerson.Address) return false;
                        break;
                    case "email":
                        if (storedPerson.Email != filterPerson.Email) return false;
                        break;
                }
            }
            return true;
        }
    }
}
