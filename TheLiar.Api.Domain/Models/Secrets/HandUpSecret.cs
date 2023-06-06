namespace TheLiar.Api.Domain.Models.Secrets;

public record HandUpSecret(string Text) : ISecret
{
    public SecretType Type => SecretType.HandUp;
}