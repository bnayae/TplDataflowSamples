using Bnaya.Samples;
using Bnaya.Samples.Hooks;
using System;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples.Hooks
{
    public interface IPropogateHook<TIn, TOut> : IPropagatorBlock<TIn, TOut>, ISourceHook<TOut>, ITargetHook<TIn>, IDataflowHook
    {
        void LinkCandidate(
            ITargetHook<TOut> targetBlockInfo, 
            DataflowLinkOptions linkOptions = null,
            Predicate<TOut> predicate = null);
    }
}
