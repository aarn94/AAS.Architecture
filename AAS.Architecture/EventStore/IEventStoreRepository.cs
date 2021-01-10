using System;
using System.Threading.Tasks;

namespace AAS.Architecture.EventStore
{
    public interface IEventStoreRepository<T> where T : IEventBaseAggregate<Guid>, new()
    {
        Task SaveAsync(IEventBaseAggregate<Guid> root, Guid commitId);

        Task SaveAsync(IEventBaseAggregate<Guid> root);

        Task<T> GetByIdAsync(Guid id);
    }
}