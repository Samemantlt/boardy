using TheLiar.Api.Domain.Models.Secrets;

namespace TheLiar.Infrastructure.Services;

public interface ISecretsContainer
{
    ValueTask<List<ISecret>> Get();
}

public abstract class FileSecretsContainer : ISecretsContainer
{
    public string FilePath { get; }


    protected FileSecretsContainer(string filePath)
    {
        FilePath = filePath;
    }


    public async ValueTask<List<ISecret>> Get()
    {
        var lines = await File.ReadAllLinesAsync(FilePath);
        return lines.Select(Parse).ToList();
    }


    protected abstract ISecret Parse(string line);
}

public class HandUpFileSecretsContainer : FileSecretsContainer
{
    public HandUpFileSecretsContainer(string filePath) : base(filePath) { }

    protected override ISecret Parse(string line) => new HandUpSecret(line);
}

public class ShowNumberFileSecretsContainer : FileSecretsContainer
{
    public ShowNumberFileSecretsContainer(string filePath) : base(filePath) { }

    protected override ISecret Parse(string line) => new ShowNumberSecret(line);
}

public class PointSomebodyFileSecretsContainer : FileSecretsContainer
{
    public PointSomebodyFileSecretsContainer(string filePath) : base(filePath) { }

    protected override ISecret Parse(string line) => new PointSomebodySecret(line);
}