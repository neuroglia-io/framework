using Microsoft.Extensions.Logging;

namespace Neuroglia.Plugins.Services;

/// <summary>
/// Represents a <see cref="NuGet.Common.ILogger"/> that leverages <see cref="ILogger"/>s
/// </summary>
public class LoggingExtensionLogger
    : NuGet.Common.LoggerBase
{

    /// <summary>
    /// Initializes a new <see cref="LoggingExtensionLogger"/>
    /// </summary>
    /// <param name="logger">The underlying <see cref="ILogger"/></param>
    public LoggingExtensionLogger(ILogger logger) { this.Logger = logger; }

    /// <summary>
    /// Gets the service used to perform logging
    /// </summary>
    protected ILogger Logger { get; }

    /// <inheritdoc/>
    public override void Log(NuGet.Common.ILogMessage message)
    {
        switch (message.Level)
        {
#pragma warning disable CA2254 // Template should be a static expression
            case NuGet.Common.LogLevel.Error: this.Logger.LogError(message.Message); break;
            case NuGet.Common.LogLevel.Information: this.Logger.LogInformation(message.Message); break;
            case NuGet.Common.LogLevel.Warning: this.Logger.LogWarning(message.Message); break;
            default: this.Logger.LogDebug(message.Message); break;
#pragma warning restore CA2254 // Template should be a static expression
        };
    }

    /// <inheritdoc/>
    public override Task LogAsync(NuGet.Common.ILogMessage message) => Task.Run(() => this.Log(message));

}