using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public abstract class PropagatorHookBase<TInput, TOutput> : IPropagatorBlock<TInput, TOutput>, IReceivableSourceBlock<TOutput>
    {
        public string Name { get; private set; }

        public IDataflowBlock Block { get; private set; }
        public ITargetBlock<TInput> Target { get; private set; }
        public ISourceBlock<TOutput> Source { get; private set; }
        public IReceivableSourceBlock<TOutput> SourceReceivable { get; private set; }

        #region Ctor

        public PropagatorHookBase(string name, IPropagatorBlock<TInput, TOutput> block)
            : this(name, block, block, block) { }

        public PropagatorHookBase(string name,
            IDataflowBlock block, 
            ITargetBlock<TInput> target, 
            ISourceBlock<TOutput> source)
        {
            Name = name;
            Block = block;
            Target = target;
            Source = source;
            SourceReceivable = source as IReceivableSourceBlock<TOutput>;
        }

        #endregion // Ctor

        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
        {
            OnBeginOfferMessage(Name, messageHeader, messageValue, source, consumeToAccept);
            DataflowMessageStatus response = Target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
            OnEndOfferMessage(Name, messageHeader, messageValue, source, consumeToAccept, response);
            return response;
        }

        public void Complete()
        {
            Block.Complete();
        }

        public Task Completion
        {
            get { return Block.Completion; }
        }

        public void Fault(Exception exception)
        {
            Block.Fault(exception);
        }

        public IDisposable LinkTo(ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
        {
            IDisposable unlink = Source.LinkTo(target, linkOptions);
            OnLinkTo(Name, target, linkOptions);

            var trigerDisposable = new CancellationDisposable();
            trigerDisposable.Token.Register(() => OnUnlinkTo(Name, target));
            var dispComposite = new CompositeDisposable(trigerDisposable, unlink);

            return dispComposite;
        }

        public TOutput ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
        {
            TOutput value = Source.ConsumeMessage(messageHeader, target, out messageConsumed);
            OnConsumeMessage(Name, messageHeader, target, messageConsumed, value);
            return value;
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            bool succeed = Source.ReserveMessage(messageHeader, target);
            OnReserveMessage(Name, messageHeader, target, succeed);
            return succeed;
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            Source.ReleaseReservation(messageHeader, target);
            OnReleaseReservation(Name, messageHeader, target);
        }

        public bool TryReceive(Predicate<TOutput> filter, out TOutput item)
        {
            bool succeed = SourceReceivable.TryReceive(filter, out item);
            OnTryReceive(Name, item, succeed);
            return succeed;
        }

        public bool TryReceiveAll(out IList<TOutput> items)
        {
            bool succeed = SourceReceivable.TryReceiveAll(out items);
            OnTryReceiveAll(Name, items, succeed);
            return succeed;
        }

		#region Interception Points

        protected virtual void OnBeginOfferMessage(string name, DataflowMessageHeader messageHeader,
            TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
        { 
        }

        protected virtual void OnEndOfferMessage(string name, DataflowMessageHeader messageHeader,
            TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept, DataflowMessageStatus response)
        { 
        }

        protected virtual void OnLinkTo(string name, ITargetBlock<TOutput> target, DataflowLinkOptions linkOptions)
        { 
        }
        
        protected virtual void OnUnlinkTo(string name, ITargetBlock<TOutput> target)
        {
        }

        protected virtual void OnConsumeMessage(string name, DataflowMessageHeader messageHeader,
            ITargetBlock<TOutput> target, bool messageConsumed, TOutput value)
        {
        }

        protected virtual void OnReserveMessage(string name, DataflowMessageHeader messageHeader, 
            ITargetBlock<TOutput> target, bool succeed)
        {
        }

        protected virtual void OnReleaseReservation(string name, DataflowMessageHeader messageHeader, 
            ITargetBlock<TOutput> target)
        {
        }

        protected virtual void OnTryReceive(string name, TOutput item, bool succeed){}

        protected virtual void OnTryReceiveAll(string name, IList<TOutput> items, bool succeed){}

		#endregion // Interception Points
    }
}
