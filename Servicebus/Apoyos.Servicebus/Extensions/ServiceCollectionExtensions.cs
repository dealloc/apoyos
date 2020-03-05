using Apoyos.Servicebus.Configuration;
using Apoyos.Servicebus.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Apoyos.Servicebus.Extensions
{
    /// <summary>
    /// Contains extension methods for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add an <see cref="IDomainEventHandler{TEvent}"/> for <typeparamref name="TEvent"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the event listener will be added.</param>
        /// <param name="name">The domain name of the event for which a listener is being added.</param>
        /// <typeparam name="TEvent">The domain event for which to register a listener.</typeparam>
        /// <typeparam name="THandler">The type of the handler to register for <typeparamref name="TEvent"/>.</typeparam>
        public static void AddDomainEventListener<TEvent, THandler>(this IServiceCollection services, string name) where TEvent : class, new() where THandler : class, IDomainEventHandler<TEvent>
        {
            services.Configure<ServicebusConfiguration>(config => config._events.Add(name, typeof(TEvent)));
            
            services.TryAddTransient<IDomainEventHandler<TEvent>, THandler>();
        }

        /// <summary>
        /// Add a domain event for which no <see cref="IDomainEventHandler{TEvent}"/> is registered (locally).
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the event listener will be added.</param>
        /// <param name="name">The domain name of the event for which a listener is being added.</param>
        /// <typeparam name="TEvent">The domain event for which to register a listener.</typeparam>
        /// <seealso cref="AddDomainEventListener{TEvent,THandler}"/>
        public static void AddDomainEvent<TEvent>(this IServiceCollection services, string name) where TEvent : class, new()
        {
            services.Configure<ServicebusConfiguration>(config => config._events.Add(name, typeof(TEvent)));
        }
    }
}