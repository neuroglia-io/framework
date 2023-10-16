using System.ComponentModel.DataAnnotations;

namespace Neuroglia.UnitTests.Data;

public class Address
{

    [Required]
    public string Street { get; set; } = null!;

    [Required]
    public string ZipCode { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;

}
