namespace Neuroglia.Data;

/// <summary>
/// Defines the fundamentals of an object that keeps track of its state's version
/// </summary>
public interface IVersionedState
{

    /// <summary>
    /// Gets the object's state version
    /// </summary>
    ulong StateVersion { get; set; }

}

