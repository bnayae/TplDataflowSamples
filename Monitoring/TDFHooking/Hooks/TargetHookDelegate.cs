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
    public class TargetHookDelegate<T> : ITargetBlock<T>
    {
        private static ConsoleColor[] _colors = new[] { ConsoleColor.Blue, ConsoleColor.Magenta };
        private static int _nextColor = 0;
        public string Name { get; private set; }
        public ITargetBlock<T> Target { get; private set; }
        public ISourceBlock<T> Source { get; private set; }
        private readonly object _sync;
        private readonly ConsoleColor _color;

        public TargetHookDelegate(string name, ISourceBlock<T> source, ITargetBlock<T> target, object sync)
        {
            Name = name;
            Source = source;
            Target = target;
            _sync = sync;
            _color = _colors[(_nextColor++) % _colors.Length];
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, T messageValue, ISourceBlock<T> source, bool consumeToAccept)
        {
            DataflowMessageStatus result = Target.OfferMessage(messageHeader, messageValue, Source, consumeToAccept);
            lock (_sync)
            {
                Console.ForegroundColor = _color;
                int count = -1;
                var b = Target as ActionBlock<T>;
                if (b != null)
                    count = b.InputCount;

                Console.WriteLine("Offer - {0}: Message Id = {1}, Value = {2}, Result = {3}",
                    Name, messageHeader.Id, messageValue, result);
                Console.ResetColor();
            }
            return result;
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
