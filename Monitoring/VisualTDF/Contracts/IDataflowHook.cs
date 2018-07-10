using Bnaya.Samples;
using System;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

namespace Bnaya.Samples.Hooks
{
    public interface IDataflowHook : IDataflowBlock
    {
        BlockInformation BlockInfo { get; }

        void AddCommand(string title, Action execute);
        void AddCommand(string title, Action execute, Color backColor);
        void AddCommand(string title, Action execute, Color backColor, Color foreColor);
    }
}
