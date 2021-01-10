namespace AAS.Architecture.Gateway.Infrastructure
{
    internal interface IAnonymousRouteValidator
    {
        bool HasAccess(string path);
    }
}