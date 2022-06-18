namespace LokiLoggingProvider.UnitTests;

using LokiLoggingProvider.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

public class LokiLoggerExtensionsUnitTests
{
    [Fact]
    public void When_AddingLokiLogger_Expect_LokiLoggerAdded()
    {
        // Arrange
        ILoggingBuilder builder = new MockLoggingBuilder();

        // Act
        builder.AddLoki();

        // Assert
        using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        Assert.NotNull(serviceProvider.GetService<IOptions<LokiLoggerOptions>>());
        Assert.NotNull(serviceProvider.GetService<IOptionsSnapshot<LokiLoggerOptions>>());
        Assert.NotNull(serviceProvider.GetService<IOptionsMonitor<LokiLoggerOptions>>());

        Assert.IsType<LokiLoggerProvider>(serviceProvider.GetService<ILoggerProvider>());

        Assert.Collection(
            builder.Services,
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(ILoggerProviderConfigurationFactory), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(ILoggerProviderConfiguration<>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptions<>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsSnapshot<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsManager<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsMonitor<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsMonitor<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsFactory<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsFactory<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsMonitorCache<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsCache<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(ILoggerProvider), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(LokiLoggerProvider), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IConfigureOptions<LokiLoggerOptions>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsChangeTokenSource<LokiLoggerOptions>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            });
    }

    [Fact]
    public void When_AddingLokiLoggerWithConfigureAction_Expect_LokiLoggerAdded()
    {
        // Arrange
        ILoggingBuilder builder = new MockLoggingBuilder();

        // Act
        builder.AddLoki(configure => configure.StaticLabels.JobName = nameof(LokiLoggerExtensionsUnitTests));

        // Assert
        using ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

        IOptions<LokiLoggerOptions> options = serviceProvider.GetService<IOptions<LokiLoggerOptions>>();
        IOptionsSnapshot<LokiLoggerOptions> optionsSnapshot = serviceProvider.GetService<IOptionsSnapshot<LokiLoggerOptions>>();
        IOptionsMonitor<LokiLoggerOptions> optionsMonitor = serviceProvider.GetService<IOptionsMonitor<LokiLoggerOptions>>();

        Assert.NotNull(options);
        Assert.NotNull(optionsSnapshot);
        Assert.NotNull(optionsMonitor);

        Assert.Equal(nameof(LokiLoggerExtensionsUnitTests), options.Value.StaticLabels.JobName);
        Assert.Equal(nameof(LokiLoggerExtensionsUnitTests), optionsSnapshot.Value.StaticLabels.JobName);
        Assert.Equal(nameof(LokiLoggerExtensionsUnitTests), optionsMonitor.CurrentValue.StaticLabels.JobName);

        Assert.IsType<LokiLoggerProvider>(serviceProvider.GetService<ILoggerProvider>());

        Assert.Collection(
            builder.Services,
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(ILoggerProviderConfigurationFactory), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(ILoggerProviderConfiguration<>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptions<>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Scoped, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsSnapshot<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsManager<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsMonitor<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsMonitor<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Transient, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsFactory<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsFactory<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsMonitorCache<>), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(OptionsCache<>), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(ILoggerProvider), serviceDescriptor.ServiceType);
                Assert.Equal(typeof(LokiLoggerProvider), serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IConfigureOptions<LokiLoggerOptions>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IOptionsChangeTokenSource<LokiLoggerOptions>), serviceDescriptor.ServiceType);
                Assert.NotNull(serviceDescriptor.ImplementationType);
            },
            serviceDescriptor =>
            {
                Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
                Assert.Equal(typeof(IConfigureOptions<LokiLoggerOptions>), serviceDescriptor.ServiceType);
                Assert.Null(serviceDescriptor.ImplementationType);
            });
    }

    private class MockLoggingBuilder : ILoggingBuilder
    {
        public IServiceCollection Services { get; } = new ServiceCollection();
    }
}
