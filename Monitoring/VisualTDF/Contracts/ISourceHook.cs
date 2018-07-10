using Bnaya.Samples;
using System;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples.Hooks
{
    public interface ISourceHook<T> : ISourceBlock<T>, IDataflowHook
    {
        ISourceBlock<T> InternalSource { get; }
    }
}
