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
    internal class PropogateHook<TIn, TOut> : SourceHook<TOut>, IPropogateHook<TIn, TOut>, IPropagatorBlock<TIn, TOut>
    {
        private readonly ITargetBlock<TIn> _target;
        // generic way to trigger refresh (can be block delegate wrapper or Observable.Interval + scheduler)

        #region Ctor

        #region Overloads

        /// <summary>
        /// Initializes a new instance of the <see cref="PropogateHook{TIn, TOut}"/> class.
        /// </summary>
        /// <param name="local">The local.</param>
        /// <param name="name">The name.</param>
        /// <param name="visitor">Central information repository</param>
        /// <param name="color"></param>
        public PropogateHook(
            IPropagatorBlock<TIn, TOut> local,
            string name,
            DataflowVisitor visitor,
            Color? color = null)
            : this(local, local, name, visitor, color)
        {
        }

        #endregion // Overloads

        /// <summary>
        /// Prevents a default instance of the <see cref="PropogateHook{TIn, TOut}"/> class from being created.
        /// </summary>
        /// <param name="source">The source block.</param>
        /// <param name="target">The target block.</param>
        /// <param name="name">The name.</param>
        /// <param name="visitor">Central information repository</param>
        /// <param name="color"></param>
        private PropogateHook(
            ISourceBlock<TOut> source,
            ITargetBlock<TIn> target, 
            string name,
            DataflowVisitor visitor,
            Color? color = null)
            : base(source, name, visitor, color)
        {
            _target = target;
        }

        #endregion // Ctor

        #region TrackProperties

        protected override void TrackProperties(ISourceBlock<TOut> source)
        {
            _reflectView = ReflectBlockFactory.Create(source as IPropagatorBlock<TIn, TOut>, BlockInfo);
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
            TIn messageValue, 
            ISourceBlock<TIn> source,
            bool consumeToAccept)
        {
            DataflowMessageStatus result =
                _target.OfferMessage(messageHeader, messageValue, source, consumeToAccept);

            BlockInfo.PushOfferingCounter.Increment(result);

            #region BlockInfo.PushOffering.Add(...)

            var hookSource = source as ISourceHook<TIn>;
            if(!(source is LinkHook<TIn>)) // optimization, in this case the like hook will handle the message
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
