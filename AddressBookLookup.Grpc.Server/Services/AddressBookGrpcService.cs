using AddressBookLookup.Grpc.Application.Interfaces;
using AddressBookLookup.Grpc.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Serilog;

namespace AddressBookLookup.Grpc.Server.Services
{
    public class AddressBookGrpcService(ILogger<AddressBookGrpcService> logger, IAddressBookService addressBookService)
        : AddressBookLookupService.AddressBookLookupServiceBase
    {
        public override async Task<AddressBook> GetAddressBook(Empty request, ServerCallContext context)
        {
            try
            {
                logger.LogInformation(" - GetAddressBook called");

                var people = await addressBookService.GetAddressBookAsync();
                var addressBook = new AddressBook();
                addressBook.People.AddRange(people);

                logger.LogInformation($" - GetAddressBook response: {addressBook}");

                return addressBook;
            }
            catch (Exception ex)
            {
                logger.LogError($" - Error occurred while processing GetAddressBook request: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }

        public override async Task FindPerson(FindPersonRequest request, IServerStreamWriter<Person> responseStream, ServerCallContext context)
        {
            try
            {
                logger.LogInformation($" - FindPerson called with filters: {request}");

                // Filtrar solo los campos indicados en el FieldMask
                var filteredPeople = await addressBookService.FindPersonAsync(request.Person, request.FieldMask);

                if (!filteredPeople.Any())
                {
                    logger.LogWarning($" - No persons found matching the filters: {request}");
                }

                logger.LogInformation($" - FindPerson response:");

                foreach (var person in filteredPeople)
                {
                    logger.LogInformation($"   {person}");
                    await responseStream.WriteAsync(person);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($" - Error occurred while processing FindPerson request: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
            }
        }
    }
}
