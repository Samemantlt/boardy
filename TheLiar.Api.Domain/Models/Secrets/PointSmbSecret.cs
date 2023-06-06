namespace TheLiar.Api.Domain.Models.Secrets;

public record PointSmbSecret(string Text) : ISecret
{
    public SecretType Type => SecretType.HandUp;
}