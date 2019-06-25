using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicTdfDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new SomeSource();
            var targetA = new ActionBlock<int>(async i =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                Console.Write($"A{i},");
            }); //, new ExecutionDataflowBlockOptions { BoundedCapacity = 10});
            var targetB = new ActionBlock<int>(i => Console.Write($"B{i},"));
            source.LinkTo(targetA, i => i % 2 == 0);
            source.LinkTo(targetB);

            Thread.Sleep(5000);
            source.Complete();
            //Task completion = source.Completion;

            Console.ReadKey();
        }
    }
}
