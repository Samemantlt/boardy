namespace TheLiar.Api.Domain.Models.Secrets;

public record PointSomebodySecret(string Text) : ISecret
{
    public SecretType Type => SecretType.PointSomebody;
}