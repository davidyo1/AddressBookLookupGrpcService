using AddressBookLookup.Grpc.Application.Interfaces;
using AddressBookLookup.Grpc.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Serilog;

namespace AddressBookLookup.Grpc.Server.Services
{
    /// <summary>
    /// Implements the AddressBookLookup gRPC service, providing methods to manage an address book.
    /// </summary>
    public class AddressBookGrpcService(ILogger<AddressBookGrpcService> logger, IAddressBookService addressBookService)
        : AddressBookLookupService.AddressBookLookupServiceBase
    {
        private const string InternalServerError = "Internal server error";
        private const string OperationCancelledMessage = "Operation was cancelled";

        /// <summary>
        /// This is a unary RPC that returns all people in the address book in a single response.
        /// </summary>
        /// <param name="request">An empty request as no parameters are needed.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>The complete address book containing all people.</returns>
        /// <exception cref="RpcException">Thrown when an error occurs during processing.</exception>
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
                throw new RpcException(new Status(StatusCode.Internal, InternalServerError));
            }
        }

        /// <summary>
        /// Searches for people matching the given criteria and streams the results.
        /// This is a server-side streaming RPC that:
        /// 1. Uses FieldMask to filter which fields to compare
        /// 2. Streams results as they are found, allowing for efficient processing of large result sets
        /// 3. Provides real-time feedback to the client
        /// 4. Handles cancellation to prevent resource leaks
        /// </summary>
        /// <param name="request">The search criteria and field mask for filtering.</param>
        /// <param name="responseStream">The stream to write matching people to.</param>
        /// <param name="context">The server call context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="RpcException">Thrown when an error occurs during processing.</exception>
        public override async Task FindPerson(FindPersonRequest request, IServerStreamWriter<Person> responseStream, ServerCallContext context)
        {
            try
            {
                logger.LogInformation($" - FindPerson called with filters: {request}");

                var filteredPeople = await addressBookService.FindPersonAsync(request.Person, request.FieldMask);

                if (!filteredPeople.Any())
                {
                    logger.LogWarning($" - No persons found matching the filters: {request}");
                }

                logger.LogInformation(" - FindPerson response:");

                foreach (var person in filteredPeople)
                {
                    // Check for cancellation before writing each person
                    context.CancellationToken.ThrowIfCancellationRequested();

                    logger.LogInformation($"   {person}");
                    await responseStream.WriteAsync(person);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation(" - FindPerson operation was cancelled by the client");
                throw new RpcException(new Status(StatusCode.Cancelled, OperationCancelledMessage));
            }
            catch (Exception ex)
            {
                logger.LogError($" - Error occurred while processing FindPerson request: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, InternalServerError));
            }
        }
    }
}
