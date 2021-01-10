using NJsonSchema.Annotations;

namespace AAS.Architecture.Gateway.Cors

{
    public class WebOptions
    {
        [CanBeNull]
        public string Url { get; set; }
    }
}