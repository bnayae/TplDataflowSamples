using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class SourceHook<T>: PropagatorHookBase<T,T>
    {
        #region Ctor

        public SourceHook(string name, IPropagatorBlock<T,T> block)
            : base(name, block)
        {
        }

        #endregion // Ctor

		#region Interception Points

        protected override void OnConsumeMessage(string name, DataflowMessageHeader messageHeader, ITargetBlock<T> target, bool messageConsumed, T value)
        {
            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Source Consume [{0}], message id = {1}, consumed = {2}, Target = {3}", name, messageHeader.Id, messageConsumed, ((TargetHook<T>)target).Name);
                Console.ResetColor();
            }
        }

        protected override void OnTryReceive(string name, T item, bool succeed)
        {
            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Source TryReceive [{0}], succeed {1}", name, succeed);
                Console.ResetColor();
            }
        }

        protected override void OnTryReceiveAll(string name, IList<T> items, bool succeed)
        {
            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Source TryReceiveAll [{0}], succeed {1}", name, succeed);
                Console.ResetColor();
            }
        }

        protected override void OnReleaseReservation(string name, DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            Console.WriteLine("OnReleaseReservation");
        }

        protected override void OnReserveMessage(string name, DataflowMessageHeader messageHeader, ITargetBlock<T> target, bool succeed)
        {
            Console.WriteLine("OnReserveMessage");
        }

		#endregion // Interception Points
    }
}
