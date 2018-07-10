using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sela.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = ProducerAsync();
            Consumer();
            t.Wait();
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static BufferBlock<int> _buffer = new BufferBlock<int>(new DataflowBlockOptions { BoundedCapacity = 2 });

        private static async Task ProducerAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                bool hasSent = await _buffer.SendAsync(i);
                if(hasSent)
                    Console.WriteLine("Sent:{0} ", i);
                else
                    Console.WriteLine("{0}: Rejected", i);
            }
            _buffer.Complete();
        }

        private static void Consumer()
        {
            while (!_buffer.Completion.IsCompleted)
            {
                int item = _buffer.Receive();
                Thread.Sleep(1000);
                Console.WriteLine("\t Received: {0}",item);
            }
        }

    }
}
