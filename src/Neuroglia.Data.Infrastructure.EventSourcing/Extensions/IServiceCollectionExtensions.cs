using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Neuroglia.Data.Infrastructure.EventSourcing.Services;
using Neuroglia.Data.Infrastructure.Services;

namespace Neuroglia.Data.Infrastructure.EventSourcing;

/// <summary>
/// Defines extensions for <see cref="IServiceCollection"/>s
/// </summary>
public static class IServiceCollectionExtensions
{

    /// <summary>
    /// Adds and configures a new <see cref="EventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="aggregateType">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="EventSourcingRepository{TAggregate, TKey}"/> for</param>
    /// <param name="keyType">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add</param>
    /// <param name="lifetime">The lifetime of the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventSourcingRepository(this IServiceCollection services, Type aggregateType, Type keyType, ServiceLifetime lifetime)
    {
        if (aggregateType == null) throw new ArgumentNullException(nameof(aggregateType));
        if (!typeof(IAggregateRoot<>).MakeGenericType(keyType).IsAssignableFrom(aggregateType)) throw new ArgumentException("The specified type must be an IAggregateRoot<TKey> implementation", nameof(aggregateType));
        if (keyType == null) throw new ArgumentNullException(nameof(keyType));
        var implementationType = typeof(EventSourcingRepository<,>).MakeGenericType(aggregateType, keyType);
        services.TryAdd(new ServiceDescriptor(implementationType, implementationType, lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IRepository<,>).MakeGenericType(aggregateType, keyType), provider => provider.GetRequiredService(implementationType), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IRepository<>).MakeGenericType(aggregateType), provider => provider.GetRequiredService(implementationType), lifetime));
        return services;
    }

    /// <summary>
    /// Adds and configures a new <see cref="EventSourcingRepository{TAggregate, TKey}"/>
    /// </summary>
    /// <typeparam name="TAggregate">The type of <see cref="IAggregateRoot{TKey}"/> to add a new <see cref="EventSourcingRepository{TAggregate, TKey}"/> for</typeparam>
    /// <typeparam name="TKey">The type of key used to uniquely identify the <see cref="IAggregateRoot"/>s managed by the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="lifetime">The lifetime of the <see cref="EventSourcingRepository{TAggregate, TKey}"/> to add. Defaults to <see cref="ServiceLifetime.Scoped"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventSourcingRepository<TAggregate, TKey>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TAggregate : class, IAggregateRoot<TKey>
        where TKey : IEquatable<TKey>
    {
        return services.AddEventSourcingRepository(typeof(TAggregate), typeof(TKey), lifetime);
    }

    /// <summary>
    /// Adds and configures a new <see cref="IEventStore"/> of the specified type
    /// </summary>
    /// <typeparam name="TEventStore">The type of <see cref="IEventStore"/> to use</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
    /// <param name="setupAction">An <see cref="Action{T}"/> used to configure the <see cref="IEventStore"/></param>
    /// <returns>The configured <see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddEventStore<TEventStore>(this IServiceCollection services, Action<IEventStoreOptionsBuilder> setupAction)
        where TEventStore : class, IEventStore
    {
        var optionsBuilder = new EventStoreOptionsBuilder();
        setupAction(optionsBuilder);
        var options = optionsBuilder.Build();
        services.TryAddSingleton(Options.Create(options));
        services.TryAddSingleton(typeof(IEventAggregatorFactory), options.AggregatorFactoryType);
        services.TryAddSingleton(typeof(IEventMigrationManager), options.MigrationManagerType);
        return services;
    }

}
