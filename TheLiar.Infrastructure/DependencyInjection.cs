using Microsoft.Extensions.DependencyInjection;
using TheLiar.Api.Domain.Services;
using TheLiar.Infrastructure.Services;

namespace TheLiar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISecretsContainer>(new HandUpFileSecretsContainer("hands_up.questions.txt"));
        services.AddSingleton<ISecretsContainer>(new PointSomebodyFileSecretsContainer("point_smb.questions.txt"));
        services.AddSingleton<ISecretsContainer>(new ShowNumberFileSecretsContainer("show_number.questions.txt"));

        services.AddSingleton<ISecretSource, SecretSource>();
        services.AddHostedService(p => (SecretSource)p.GetRequiredService<ISecretSource>());
        
        return services;
    }
}