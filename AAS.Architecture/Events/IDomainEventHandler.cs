using System.Threading.Tasks;

namespace AAS.Architecture.Events
{
    public interface IDomainEventHandler<in T> where T : class, IDomainEvent
    {
        Task HandleAsync(T @event);
    }
}