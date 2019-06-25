using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JoinSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var b1 = new BufferBlock<int>();
            var b2 = new BufferBlock<string>();

            var j = new JoinBlock<int, string>();

            var a = new ActionBlock<Tuple<int, string>>(x => Console.WriteLine(x));

            b1.LinkTo(j.Target1);
            b2.LinkTo(j.Target2);

            j.LinkTo(a);

            for (int i = 0; i < 10; i++)
            {
                b1.Post(i);
            }
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(500);
                b2.Post(new string('*', i + 1));
            }

            Console.ReadKey();
        }
    }
}
