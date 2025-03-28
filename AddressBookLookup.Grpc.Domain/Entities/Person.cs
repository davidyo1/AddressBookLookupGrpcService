namespace AddressBookLookup.Grpc.Domain.Entities
{
    public class Person
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public Person() { }
    }
}
