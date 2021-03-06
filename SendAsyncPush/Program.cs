﻿using System;

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
        private static ActionBlock<int> _ab;
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            _ab = new ActionBlock<int>(async i =>
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    Console.WriteLine("\r\n{0}: Handled", i);
                },
                new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 5 ,
                    //CancellationToken = cts.Token
                });

           
            #region Remarked

            //cts.Token.Register(() => _ab.Complete());

            Task t = GenerateDataPitfallAsync(cts.Token);
            //Task t = GenerateDataAsync(cts.Token);

            #endregion // Remarked
            //Task t = GenerateDataAsync();

            //int j = 0;
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
                //if (j++ > 50)
                //    _ab.Complete();
            }
            //while (!_ab.Completion.IsCompleted)
            //{
            //    Console.Write("- ");
            //    Thread.Sleep(100);
            //}
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static async Task GenerateDataAsync()
        {
            for (int i = 0; i < 100; i++)
            {
                // load data 5M

                Console.WriteLine();
                if(await _ab.SendAsync(i).ConfigureAwait(false)) 
                    Console.WriteLine("{0}: Sent", i);
                else
                    Console.WriteLine("{0}: Rejected", i);
            }
            _ab.Complete();
        }

        private static async Task GenerateDataPitfallAsync(CancellationToken ct)
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(); 
                if (await _ab.SendAsync(i, ct).ConfigureAwait(false)) // don't send if canceled
                    Console.WriteLine("{0}: Sent", i);
                else
                    Console.WriteLine("{0}: Rejected", i);
            }
            _ab.Complete();
        }

        private static async Task GenerateDataAsync(CancellationToken ct)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine();
                    if (await _ab.SendAsync(i, ct).ConfigureAwait(false)) // Hang send if canceled
                        Console.WriteLine("{0}: Sent", i);
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("\r\nCANCELED !!!!");
            }
            finally
            {
                _ab.Complete();
            }
        }
    }
}
