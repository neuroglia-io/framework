namespace Neuroglia.UnitTests.Data
{
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.AbstractClassConverterFactory))]
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.AbstractClassConverterFactory))]
    [Discriminator(nameof(Discriminator))]
    public abstract class Abstraction
    {

        public abstract string Discriminator { get; }

    }

}
