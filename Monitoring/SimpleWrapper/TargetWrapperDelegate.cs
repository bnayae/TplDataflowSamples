using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class TargetWrapperDelegate<TInput, TOutput>: IPropagatorBlock<TInput,TOutput>
    {
        public string Name { get; private set; }

        public IDataflowBlock Block { get; private set; }
        public ITargetBlock<TInput> Target { get; private set; }
        public ISourceBlock<TOutput> Source { get; private set; }

        //  name, messageHeader, messageValue, source, consumeToAccept, response
        private Action<string, DataflowMessageHeader, TInput, ISourceBlock<TInput>, bool, DataflowMessageStatus> _onOfferMessage;
        //  name, target, linkOptions
        private Action<string, ITargetBlock<TOutput>, DataflowLinkOptions> _onLinkTo;
        //  name, target
        private Action<string, ITargetBlock<TOutput>> _onUnlinkTo;
        //  name, messageHeader, target, messageConsumed, value
        private Action<string, DataflowMessageHeader, ITargetBlock<TOutput>, bool, TOutput> _onConsumeMessage;
        //  name, messageHeader, target, succeed
        private Action<string, DataflowMessageHeader, ITargetBlock<TOutput>, bool> _onReserveMessage;
        //  name, messageHeader, target
        private Action<string, DataflowMessageHeader, ITargetBlock<TOutput>> _onReleaseReservation;

        public TargetWrapperDelegate(string name, IPropagatorBlock<TInput, TOutput> block)
        {
            Name = name;
            Block = block;
            Target = block;
            Source = block;
        }

        public TargetWrapperDelegate(string name, ITargetBlock<TInput> target)
        {
            Name = name;
            Block = target;
            Target = target;
        }

        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader, TInput messageValue, ISourceBlock<TInput> source, bool consumeToAccept)
        {
            DataflowMessageStatus response = Target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
            _onOfferMessage(Name, messageHeader, messageValue, source, consumeToAccept, response);
            return response;
        }

        public TargetWrapperDelegate(string name, ISourceBlock<TOutput> source)
        {
            Name = name;
            Block = source;
            Source = source;
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
            _onLinkTo(Name, target, linkOptions);

            var trigerDisposable = new CancellationDisposable();
            trigerDisposable.Token.Register(() => _onUnlinkTo(Name, target));
            var dispComposite = new CompositeDisposable(trigerDisposable, unlink);

            return dispComposite;
        }

        public TOutput ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target, out bool messageConsumed)
        {
            TOutput value = Source.ConsumeMessage(messageHeader, target, out messageConsumed);
            _onConsumeMessage(Name, messageHeader, target, messageConsumed, value);
            return value;
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            bool succeed = Source.ReserveMessage(messageHeader, target);
            _onReserveMessage(Name, messageHeader, target, succeed);
            return succeed;
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOutput> target)
        {
            Source.ReleaseReservation(messageHeader, target);
            _onReleaseReservation(Name, messageHeader, target);
        }
    }
}
