namespace AddressBookLookup.Grpc.Application.Mapper
{
    public static class PersonMapperExtension
    {
        public static Domain.Entities.Person ToDomainEntity(this Common.Person person)
        {
            return new Domain.Entities.Person() { Id = 0, Name = person.Name, Address = person.Address, Email = person.Email };
        }

        public static Common.Person ToProtoEntity(this Domain.Entities.Person person)
        {
            return new Common.Person() { Name = person.Name, Address = person.Address, Email = person.Email };
        }
    }
}
