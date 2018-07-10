#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Reactive;
using System.Reactive.Linq;

#endregion // Using

namespace Sela.aSamples
{
    class Program
    {
        static void Main(string[] args)
        {
           // TdfRxSimple();
            TdfRxGroupBy();

            Console.ReadKey();
        }

        private static void TdfRxSimple()
        {
            var buffer = new BufferBlock<int>();
            Task.Run(() =>
            {
                for (int i = 0; i < 25; i++)
                {
                    buffer.Post(i);
                    Thread.Sleep(500);
                }
            });

            Console.WriteLine("Start");
            Thread.Sleep(3000);
            buffer.AsObservable()
                .Subscribe(v => Console.Write("{0},", v));
        }

        private static void TdfRxGroupBy()
        {
            var buffer = new BufferBlock<int>();
            
            Task.Run(() =>
            {
                for (int i = 0; i < 25; i++)
                {
                    buffer.Post(i);
                    Thread.Sleep(500);
                }
            });

            Console.WriteLine("Start");
            Thread.Sleep(3000);
            var groups = from item in buffer.AsObservable()
                         group item by item % 5 into g
                         select g;

            groups.Subscribe(g =>
                {
                    ConsoleColor c = (ConsoleColor)(g.Key + 10);
                    var ab = new ActionBlock<int>(item =>
                        {
                            lock (groups)
                            {
                                Console.ForegroundColor = c;
                                Console.Write("{0},", item);
                            }
                        });
                    g.Subscribe(ab.AsObserver());
                });
        }
    }
}
