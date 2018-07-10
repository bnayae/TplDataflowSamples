using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    class Program
    {
        public static readonly object _sync = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            char c = ' ';

            var options = new ExecutionDataflowBlockOptions{ BoundedCapacity = 2 };
            var act1 = new ActionBlock<int>(i =>
                {
                    Write(i, "A", 1000).Wait();
                }, options);
            //var act2 = new ActionBlock<int>(async i =>
            //{
            //    await Write(i, "B", 1000);
            //}, options);
            //var act1 = new CustomTarget("A", () => c == '1' || c == '3');
            //var act2 = new CustomTarget("B", () => c == '2' || c == '3');
            var hook1 = new TargetHook<int>("Target 1", act1);
            //var hook2 = new TargetHook<int>("Target 2", act2);

            //var buffer = new BufferBlock<int>();
            var buffer = new BroadcastBlock<int>(i => i);
            var bufferHook = new SourceHook<int>("Buffer", buffer);
            bufferHook.LinkTo(hook1);
           // bufferHook.LinkTo(hook2);

            Console.WriteLine("1 - accept A");
            Console.WriteLine("2 - accept B");
            Console.WriteLine("3 - accept AB");
            Console.WriteLine();

            int v = 0;
            while (true)
            {
                c = Console.ReadKey(true).KeyChar;
                bufferHook.SendAsync(++v);
                Trace.WriteLine("Action Count = " + act1.InputCount);
                //hook1.SendAsync(++v);
                //hook1.OfferMessage(new DataflowMessageHeader(++v), v, null, false);
            }
        }

        private static async Task Write(int i, string from, int duration)
        {
            await Task.Delay(duration);
            lock (_sync)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\tExecute [{0}]: {1}", from, i);
                Console.ResetColor();
            }
        }
    }
}
