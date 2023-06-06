namespace TheLiar.Api.Domain.Models.Secrets;

public record ShowNumberSecret(string Text) : ISecret
{
    public SecretType Type => SecretType.ShowNumber;
}