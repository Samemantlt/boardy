using TheLiar.Api.Domain.Models.Secrets;

namespace TheLiar.Api.Domain.Services;

public interface ISecretSource
{
    ISecret RandomSecret();
    
    Task Reload();
}