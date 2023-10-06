namespace Neuroglia;

/// <summary>
/// Exposes default Neuroglia error types
/// </summary>
public static class ErrorTypes
{

    static readonly Uri BaseUri = new("https://neuroglia.io/docs/errors/");

    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing an invalid request
    /// </summary>
    public static readonly Uri Invalid = new(BaseUri, "invalid");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a conflict
    /// </summary>
    public static readonly Uri Conflict = new(BaseUri, "conflict");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to find a resource
    /// </summary>
    public static readonly Uri NotFound = new(BaseUri, "not-found");
    /// <summary>
    /// Gets the <see cref="Uri"/> that references a problem describing a failure to patch a resource
    /// </summary>
    public static readonly Uri NotModified = new(BaseUri, "not-modified");

}