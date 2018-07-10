using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples.Hooks
{
    [DebuggerDisplay("{Name}")]
    public class PropogateHook<TIn, TOut> : IPropagatorBlock<TIn, TOut>
    {
        public string Name { get; private set; }
        private readonly IPropagatorBlock<TIn, TOut> _local;
        private readonly object _sync;

        public PropogateHook(string name, IPropagatorBlock<TIn, TOut> local, object sync)
        {
            Name = name;
            _local = local;
            _sync = sync;
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, TIn messageValue, ISourceBlock<TIn> source, bool consumeToAccept)
        {
            return _local.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public TOut ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOut> target, out bool messageConsumed)
        {
            TOut result = _local.ConsumeMessage(messageHeader, target, out messageConsumed);
            lock (_sync)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Consume - {0}: Message Id = {1}, Succeed = {2}, Result = {3}",
                    Name, messageHeader.Id, messageConsumed, messageConsumed ? result.ToString() : "Not available");
                Console.ResetColor();
            }

            return result;
        }

        public IDisposable LinkTo(ITargetBlock<TOut> target, DataflowLinkOptions linkOptions)
        {
            target = new TargetHookDelegate<TOut>(Name + " -> " + target.GetType().Name, this, target, _sync);
            return _local.LinkTo(target);
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<TOut> target)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<TOut> target)
        {
            throw new NotImplementedException();
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public Task Completion
        {
            get { throw new NotImplementedException(); }
        }

        public void Fault(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
