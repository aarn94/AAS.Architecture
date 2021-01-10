using JetBrains.Annotations;

namespace AAS.Architecture.Services
{
    public interface IRandomTextGenerator
    {
        [NotNull]
        string Generate(int length, [NotNull] string availableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
    }
}