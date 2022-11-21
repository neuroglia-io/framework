namespace Neuroglia.UnitTests.Data
{
    [DiscriminatedByDefault]
    public class ExtensionConcretion
    : Abstraction
    {
        public override string Discriminator => "foobar";

        public string Value { get; set; }

    }

}
