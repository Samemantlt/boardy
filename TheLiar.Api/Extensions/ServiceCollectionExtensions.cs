using MediatR;

namespace TheLiar.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLazy<T>(this IServiceCollection services) where T : notnull
    {
        services.AddSingleton<Func<T>>(p => p.GetRequiredService<T>);
        return services;
    }
    
    public static void RegisterPolymorphicNotificationHandler<TNotification, THandler>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TNotification : INotification
        where THandler : INotificationHandler<TNotification>
    {
        var notificationTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(TNotification).IsAssignableFrom(type));

        foreach (var notificationType in notificationTypes)
        {
            var type = typeof(INotificationHandler<>).MakeGenericType(notificationType);
            services.Add(new ServiceDescriptor(type, typeof(THandler), serviceLifetime));
        }
    }
}