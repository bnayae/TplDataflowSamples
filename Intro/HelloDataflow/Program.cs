using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace HelloDataflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread id:{0}", Thread.CurrentThread.ManagedThreadId);
            var broadcaster = new BroadcastBlock<int>((i) => i);

            var traceTarget = new ActionBlock<int>((i) => Trace.WriteLine(i));  // string.Format("Value: {0}, thread id: {1}", i, Thread.CurrentThread.ManagedThreadId)));
            var consoleTarget = new ActionBlock<int>((i) => Console.WriteLine(i)); // "Value: {0}, thread id: {1}", i, Thread.CurrentThread.ManagedThreadId));

            broadcaster.LinkTo(traceTarget); 
            broadcaster.LinkTo(consoleTarget);

            for (int i = 0; i < 10; i++)
                broadcaster.Post(i);

            Console.ReadKey();
        }
    }
}
