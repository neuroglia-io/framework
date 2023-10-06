namespace Neuroglia.Data;

/// <summary>
/// Represents a <see cref="DomainException"/> thrown whenever the validation of an entity has failed
/// </summary>
public class DomainValidationException
    : DomainException
{

    /// <summary>
    /// Initializes a new <see cref="DomainValidationException"/>
    /// </summary>
    /// <param name="errors">An array containing the <see cref="Error"/>s that describe the validation failures</param>
    public DomainValidationException(params Error[] errors) => this.ValidationErrors = errors;

    /// <summary>
    /// Gets an <see cref="IReadOnlyCollection{T}"/> containing the <see cref="Error"/>s that describe the validation failures
    /// </summary>
    public IReadOnlyCollection<Error> ValidationErrors { get; }

}
