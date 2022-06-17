namespace LokiLoggingProvider;

using System;
using LokiLoggingProvider.Extensions;
using LokiLoggingProvider.LoggerFactories;
using LokiLoggingProvider.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[ProviderAlias("Loki")]
public sealed class LokiLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly IDisposable onChangeToken;

    private bool disposed;

    private ILokiLoggerFactory loggerFactory;

    public LokiLoggerProvider(IOptionsMonitor<LokiLoggerOptions> options)
    {
        this.loggerFactory = options.CurrentValue.CreateLoggerFactory();

        this.onChangeToken = options.OnChange(updatedOptions =>
        {
            this.loggerFactory.Dispose();
            this.loggerFactory = updatedOptions.CreateLoggerFactory();
        });
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (this.disposed)
        {
            throw new ObjectDisposedException(nameof(LokiLoggerProvider));
        }

        return this.loggerFactory.CreateLogger(categoryName);
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.onChangeToken.Dispose();
        this.loggerFactory.Dispose();

        this.disposed = true;
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.loggerFactory.SetScopeProvider(scopeProvider);
    }
}
