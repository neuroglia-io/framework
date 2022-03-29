namespace Neuroglia.UnitTests.Data
{
    [DiscriminatorValue(nameof(Concretion))]
    public class Concretion
        : Abstraction
    {
        public override string Discriminator => nameof(Concretion);

        public string Value { get; set; }

    }

}
