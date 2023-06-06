using Microsoft.Extensions.Hosting;
using TheLiar.Api.Domain.Models.Secrets;
using TheLiar.Api.Domain.Services;

namespace TheLiar.Infrastructure.Services;

public class SecretSource : BackgroundService,  ISecretSource
{
    private readonly ISecretsContainer[] _secretsContainers;

    
    public SecretSource(IEnumerable<ISecretsContainer> secretsContainers)
    {
        _secretsContainers = secretsContainers.ToArray();
    }
    
    
    public ISecret RandomSecret()
    {
        return _secrets[_random.Next(0, _secrets.Count)];
    }

    public async Task Reload()
    {
        var secrets = new List<ISecret>();
        
        foreach (var secretsContainer in _secretsContainers)
        {
            secrets.AddRange(await secretsContainer.Get());
        }

        _secrets = secrets.AsReadOnly();
    }
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Reload();
    }

    
    private readonly Random _random = new Random();
    private IReadOnlyList<ISecret> _secrets = new List<ISecret>().AsReadOnly();
}