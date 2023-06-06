namespace TheLiar.Api.Domain.Models.Secrets;

public interface ISecret
{
    string Text { get; }
    SecretType Type { get; }
}