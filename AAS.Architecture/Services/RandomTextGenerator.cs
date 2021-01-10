
using System;

namespace AAS.Architecture.Services
{
    internal sealed class RandomTextGenerator: IRandomTextGenerator
    {
        public string Generate(int length,
            string availableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            var stringChars = new char[length];
            var random = new Random();
            
            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = availableCharacters[random.Next(availableCharacters.Length)];
            }

            return new string(stringChars);
        }
        
    }
}