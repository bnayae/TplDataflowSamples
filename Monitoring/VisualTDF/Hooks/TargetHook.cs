#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples.Hooks
{
    [DebuggerDisplay("{BlockInfo.Name}")]
    internal class TargetHook<T> : HookBase, ITargetHook<T>, ITargetBlock<T>
    {
        private readonly ITargetBlock<T> _target;
        // generic way to trigger refresh (can be block delegate wrapper or Observable.Interval + scheduler)

        #region Ctor

        public TargetHook(
            ITargetBlock<T> target, 
            string name,
            DataflowVisitor visitor,
            Color? color = null)
            : base(name, target, visitor, color)
        {
            _target = target;
            TrackProperties(target);
        }

        #endregion // Ctor

        #region TrackProperties

        protected virtual void TrackProperties(ITargetBlock<T> target)
        {
            _reflectView = ReflectBlockFactory.Create<T>(target, BlockInfo);
        }

        #endregion // TrackProperties

        #region OfferMessage (hook)

        /// <summary>
        /// Offers the message (belong to the target).
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="messageValue">The message value.</param>
        /// <param name="source">The source.</param>
        /// <param name="consumeToAccept">if set to <c>true</c> [consume to accept].</param>
        /// <returns></returns>
        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader, 
            T messageValue, 
            ISourceBlock<T> source,
            bool consumeToAccept)
        {
            DataflowMessageStatus result =
                _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);

            BlockInfo.PushOfferingCounter.Increment(result);

            #region BlockInfo.PushOffering.Add(...)

            var hookSource = source as ISourceHook<T>;
            if(!(source is LinkHook<T>)) // optimization, in this case the like hook will handle the message
            {
                string sourceName = string.Empty;
                if(hookSource != null)
                    sourceName = hookSource.BlockInfo.Name;

                var msg = new OfferMessageTrace(messageHeader.Id, messageValue, consumeToAccept, result, sourceName, BlockInfo.Name);
                BlockInfo.PushOffering.Add(msg);
            }

            #endregion // BlockInfo.PushOffering.Add(...)

            BlockInfo.Refresh();

            return result;
        }

        #endregion // OfferMessage (hook)
    }
}
