namespace Neuroglia.UnitTests.Services;

internal static class AddressFactory
{

    internal static Address Create() => new()
    {
        Street = "112 Mercer",
        City = "Princeton",
        ZipCode = "NJ 08540",
        Country = "USA"
    };

}
