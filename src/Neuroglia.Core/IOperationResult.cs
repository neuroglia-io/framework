namespace Neuroglia;

/// <summary>
/// Defines the fundamentals of an object used to describe the result of an operation
/// </summary>
public interface IOperationResult
{

    /// <summary>
    /// Gets a value that describes the status of the operation result. A value between 199 and 300 indicates success
    /// </summary>
    int Status { get; }

    /// <summary>
    /// Gets the data, if any, returned by the operation, in case of success
    /// </summary>
    object? Data { get; }

    /// <summary>
    /// Gets a list containing the errors that have occured, if any, during the execution of the operation
    /// </summary>
    IReadOnlyCollection<Error>? Errors { get; }

}

/// <summary>
/// Defines the fundamentals of an object used to describe the result of an operation
/// </summary>
/// <typeparam name="T">The type of data, if any, returned by the operation</typeparam>
public interface IOperationResult<T>
    : IOperationResult
{

    /// <summary>
    /// Gets the data, if any, returned by the operation, in case of success
    /// </summary>
    new T? Data { get; }

}
