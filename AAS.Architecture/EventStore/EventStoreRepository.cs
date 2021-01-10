using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using NEventStore;

namespace AAS.Architecture.EventStore
{
    internal sealed class EventStoreRepository<T> : IEventStoreRepository<T> where T :  IEventBaseAggregate<Guid>, new()
    {
        private readonly IEventStoreContext context;

        public EventStoreRepository(IEventStoreContext context) => this.context = context;

        public Task SaveAsync(IEventBaseAggregate<Guid> root, Guid commitId)
        {
            return ExecuteAsync();

            Task ExecuteAsync() =>
                Task.Run(() =>
                {
                    using var scope = new TransactionScope();
                    using var eventStore = context.Events;
                    using var stream = eventStore.OpenStream(root.Id, 0, int.MaxValue);

                    while (true)
                    {
                        try
                        {
                            var events = root.GetUncommittedEvents();
                            foreach (var e in events)
                                stream.Add(new EventMessage {Body = e});

                            stream.CommitChanges(commitId);
                            root.ClearUncommittedEvents();

                            scope.Complete();
                        }
                        catch (DuplicateCommitException)
                        {
                            stream.ClearChanges();
                            // Issue: #4 and test: when_an_aggregate_is_persisted_using_the_same_commitId_twice
                            // should we rethtow the exception here? or provide a feedback whether the save was successful ?
                            return;
                        }
                        catch (ConcurrencyException e)
                        {
                            stream.ClearChanges();
                            throw;
                        }
                    }
                });
        }
        
        public Task SaveAsync(IEventBaseAggregate<Guid> root)
        {
            var guid = Guid.NewGuid();
            return SaveAsync(root, guid);
        }
        
        public Task<T> GetByIdAsync(Guid id)
        {
            return ExecuteAsync();

            Task<T> ExecuteAsync() =>
                Task.Run(() =>
                {
                    var obj = new T();

                    using (var eventStore = context.Events)
                    {
                        var snapshot = eventStore.Advanced.GetSnapshot(id, int.MaxValue);
                        if (snapshot == null)
                        {
                            using var stream = eventStore.OpenStream(id, 0, int.MaxValue);
                            var events = from s in stream.CommittedEvents
                                select s.Body as ISyncDomainEvent;

                            obj.LoadFromHistory(events);
                        }
                        else
                        {
                            obj = (T) snapshot.Payload;
                            using var stream = eventStore.OpenStream(snapshot, int.MaxValue);
                            var events = from s in stream.CommittedEvents
                                select s.Body as ISyncDomainEvent;

                            obj.LoadFromHistory(events);
                        }
                    }

                    return obj;
                });
        }
    }
}