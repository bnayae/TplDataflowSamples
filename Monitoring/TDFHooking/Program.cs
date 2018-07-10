using Bnaya.Samples.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    class Program
    {
        private static readonly object _sync = new object();
        static void Main(string[] args)
        {
            Console.WindowWidth = 100;
            Console.WindowHeight += 8;
            var sync = new ManualResetEventSlim();
            var options = new ExecutionDataflowBlockOptions { BoundedCapacity = 3 };

            IPropagatorBlock<int, int> bb = new BufferBlock<int>();
            bb = new PropogateHook<int, int>("Buffer", bb, _sync);

            ITargetBlock<int> ab = new ActionBlock<int>(i =>
            {
                sync.Wait();
                Console.WriteLine("Action Block: Processing Value = {0}", i);
            }, options);

            bb.LinkTo(ab);
            var linkageToNullTarget = bb.LinkTo(DataflowBlock.NullTarget<int>());

            Console.WriteLine(@"                    |");
            Console.WriteLine(@"                   \|/");
            Console.WriteLine(@"             BufferBlock");
            Console.WriteLine(@"              |        |");
            Console.WriteLine(@"             \|/      \|/");
            Console.WriteLine(@"      ActionBlock     NullTarget");
            Console.WriteLine(@"");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("now we will offer 4 messages.\r\n");
            Console.ResetColor();

            Console.WriteLine("press any key to continue.......\r\n");
            Console.ReadKey(true);

            int item = 0;
            for (int i = 0; i < 4; i++)
            {
                item++;
                var header = new DataflowMessageHeader(item);
                (bb as ITargetBlock<int>).OfferMessage(header, item, null, false);
            }

            Thread.Sleep(100);
            Console.WriteLine("\r\n#########################################");
            Console.WriteLine("4 items was offered to the Action Block");
            Console.WriteLine("the Action Block postponed the last one");
            Console.WriteLine("because it reached its Bounded Capacity.");
            Console.WriteLine("\r\nthe item was offered to the NullTarget");
            Console.WriteLine("which accept it. now the item is no longer ");
            Console.WriteLine("available on the buffer output queue.");
            Console.WriteLine("#########################################\r\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("now we will let the Action Block processing");
            Console.WriteLine("all the items in its input queue.\r\n");
            Console.ResetColor();

            Console.WriteLine("press any key to continue.......\r\n");
            Console.ReadKey(true);

            sync.Set();
            Thread.Sleep(100);
            Console.WriteLine("\r\n#########################################");
            Console.WriteLine("the Action Block processed all the item in its input queue.");
            Console.WriteLine("(you may notice, that it try to consume message 4");
            Console.WriteLine("just when it complete to process the first message).");
            Console.WriteLine("Right now the Action Block has noting in its input queue,");
            Console.WriteLine("so theoretically it should accept new offering.");
            Console.WriteLine("#########################################\r\n");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("next we will offer 2 more items");
            Console.ResetColor();

            Console.WriteLine("press any key to continue.......\r\n");
            Console.ReadKey(true);

            for (int i = 0; i < 2; i++)
            {
                item++;
                var header = new DataflowMessageHeader(item);
                (bb as ITargetBlock<int>).OfferMessage(header, item, null, false);
            }

            Thread.Sleep(100);
            Console.ResetColor();
            Console.WriteLine("\r\n#########################################");
            Console.WriteLine("the Action Block postponed the message");
            Console.WriteLine("and try to get it via consume (in instead ");
            Console.WriteLine("of accepting the offering)");
            Console.WriteLine("in the mean time the message accepted");
            Console.WriteLine("by the NullTarget and no longer available to be consumed.");
            Console.WriteLine("\r\nTHIS IS SUPPRISING BEHAVIOR, ");
            Console.WriteLine("\r\nthe action block will never get any item");
            Console.WriteLine("unless the NullTarget will be unlinked");
            Console.WriteLine("#########################################\r\n");


            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("next we will unlink the NullTarget");
            Console.WriteLine("and then offer 2 messages\r\n");
            Console.ResetColor();

            Console.WriteLine("press any key to continue.......\r\n");
            Console.ReadKey(true);

            linkageToNullTarget.Dispose();
            Thread.Sleep(100);
            for (int i = 0; i < 2; i++)
            {
                item++;
                var header = new DataflowMessageHeader(item);
                (bb as ITargetBlock<int>).OfferMessage(header, item, null, false);
            }
            Thread.Sleep(500);

            Console.WriteLine("\r\n#########################################");
            Console.WriteLine("Summary: this behavior is not exactly what you would expected,");
            Console.WriteLine("by drilling down in to the IL, we found that");
            Console.WriteLine("the value of OutstandingTransfers in the following statement");
            Console.WriteLine("... || m_boundingState.OutstandingTransfers == 0 && ...");
            Console.WriteLine("is 1 therefore the action block is taking a wired route");
            Console.WriteLine("#########################################\r\n");

            Console.ReadKey(true);
        }
    }
}
