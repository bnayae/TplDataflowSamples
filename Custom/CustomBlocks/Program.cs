#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

#endregion // Using

namespace TDF.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            ITargetBlock<long> consumer = new Consumer();
            ISourceBlock<long> producer = new Producer();

            producer.LinkTo(consumer);

            Thread.Sleep(5000);
            producer.Complete();        // stop the provider 
            producer.Completion.Wait(); // wait until stopping

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
