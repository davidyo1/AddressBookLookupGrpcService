using AddressBookLookup.Grpc.Common;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

class Program
{
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7031");
        var client = new AddressBookLookupService.AddressBookLookupServiceClient(channel);

        var response = await client.GetAddressBookAsync(new Empty());

        Console.WriteLine(" - GetAddressBook:");
        foreach (var person in response.People)
        {
            Console.WriteLine($"   {person.Name} ({person.Email}) {person.Address}");
        }

        Console.WriteLine(" - FindPerson:");

        var request = new FindPersonRequest
        {
            Person = new Person { Address = "Calle Falsa, 456" },
            FieldMask = new FieldMask { Paths = { nameof(Person.Address) } }
        };

        using var call = client.FindPerson(request);

        await foreach (var person in call.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine($"   {person.Name} ({person.Email}) {person.Address}");
        }
        Console.ReadLine();
    }
}