﻿namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Provides mechanisms to handle positioning/offsetting in cloud event streams
/// </summary>
public static class StreamPosition
{

    /// <summary>
    /// Specifies the start of the stream
    /// </summary>
    public const long StartOfStream = 0;

    /// <summary>
    /// Specifies the end of the stream
    /// </summary>
    public const long EndOfStream = -1;

}