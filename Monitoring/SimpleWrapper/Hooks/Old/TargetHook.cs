using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class TargetHook<T>: PropagatorHookBase<T,T>
    {
        #region Ctor

        public TargetHook(string name, ITargetBlock<T> target)
            : base(name, target, target, null)
        {
        }

        #endregion // Ctor

		#region Interception Points

        protected override void OnBeginOfferMessage(string name, DataflowMessageHeader messageHeader,
            T messageValue, ISourceBlock<T> source, bool consumeToAccept)
        {
            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("tOffered Befin: [{0}], message id = {1}, push = {2}",
                    name, messageHeader.Id, !consumeToAccept);
                Console.ResetColor();
            }
        }

        protected override void OnEndOfferMessage(string name, DataflowMessageHeader messageHeader,
            T messageValue, ISourceBlock<T> source, bool consumeToAccept, DataflowMessageStatus response)
        {
            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("tOffered End: [{0}], message id = {1}, response = {2}, push = {3}", name, messageHeader.Id, response, !consumeToAccept);
                Console.ResetColor();
            }
        }

		#endregion // Interception Points
    }
}
