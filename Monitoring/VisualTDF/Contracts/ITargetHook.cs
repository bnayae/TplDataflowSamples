using Bnaya.Samples;
using System;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples.Hooks
{
    public interface ITargetHook<T> : ITargetBlock<T>, IDataflowHook
    {
    }
}
