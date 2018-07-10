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
    //[DebuggerDisplay("{BlockInfo.Name}")]
    internal class SourceHook<T> : HookBase, ISourceHook<T>, ISourceBlock<T>
    {
        // generic way to trigger refresh (can be block delegate wrapper or Observable.Interval + scheduler)
        #region Ctor

        public SourceHook(
            ISourceBlock<T> source,
            string name,
            DataflowVisitor visitor,
            Color? color = null)
            :base(name, source, visitor, color)
        {
            TrackProperties(source);
        }

        #endregion // Ctor

        private ISourceBlock<T> Source { get {return _block as ISourceBlock<T>;}}

        #region TrackProperties

        protected virtual void TrackProperties(ISourceBlock<T> source)
        {
            _reflectView = ReflectBlockFactory.Create<T>(source, BlockInfo);
        }

        #endregion // TrackProperties

        #region ConsumeMessage (hook)

        /// <summary>
        /// Consumes the message (request by other targets).
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="target">The target.</param>
        /// <param name="messageConsumed">if set to <c>true</c> [message consumed].</param>
        /// <returns></returns>
        public T ConsumeMessage(
            DataflowMessageHeader messageHeader, 
            ITargetBlock<T> target, 
            out bool messageConsumed)
        {
            T result = Source.ConsumeMessage(messageHeader, target, out messageConsumed);

            BlockInfo.PoolRequestCounters.Increment(messageConsumed);

            #region BlockInfo.PoolRequest.Add(...)

            var hookTarget = target as ITargetHook<T>;
            if (!(target is LinkHook<T>)) // optimization, in this case the like hook will handle the message
            {
                string targetName = string.Empty;
                if (hookTarget != null)
                    targetName = hookTarget.BlockInfo.Name;

                var msg = new ConsumeTrace(messageHeader.Id, messageConsumed, BlockInfo.Name, targetName, result);
                BlockInfo.PoolRequest.Add(msg);
            }

            #endregion // BlockInfo.PoolRequest.Add(...)

            BlockInfo.Refresh();

            return result;
        }

        #endregion // ConsumeMessage (hook)

        #region ReleaseReservation (hook)

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            Source.ReleaseReservation(messageHeader, target);
            BlockInfo.PoolRequestCounters.IncrementReleaseReservation();
        }

        #endregion // ReleaseReservation (hook)

        #region ReserveMessage (hook)

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            bool result = Source.ReserveMessage(messageHeader, target);
            BlockInfo.PoolRequestCounters.IncrementReserve();
            return result;
        }

        #endregion // ReserveMessage (hook)

        #region LinkTo

        /// <summary>
        /// Links to.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="linkOptions">The link options.</param>
        /// <returns></returns>
        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            return LinkTo(target, linkOptions, null);
        }

        /// <summary>
        /// Links the source to the specified target
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="linkOptions">The link options.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public IDisposable LinkTo(
            ITargetBlock<T> target,
            DataflowLinkOptions linkOptions,
            Predicate<T> predicate)
        {
            if (!BlockInfo.IsSourceBlock)
                throw new Exception("Only source block can linked to");

            var targetHook = target as ITargetHook<T>;
            LinkCandidateInformation<T> candidate = null;
            if(targetHook != null)
                candidate = new LinkCandidateInformation<T>(_visitor, this, targetHook, linkOptions, predicate);
            var hook = new LinkHook<T>(this, target, linkOptions, _visitor, predicate, candidate);
            _visitor.AddLink(hook.LinkInfo);
            return hook;
        }

        #endregion // LinkTo

        #region LinkCandidate

        public void LinkCandidate(
            ITargetHook<T> target,
            DataflowLinkOptions options = null,
            Predicate<T> predicate = null)
        {
            if (!BlockInfo.IsSourceBlock)
                throw new Exception("Only source block can linked candidates");

            var candidate = new LinkCandidateInformation<T>(_visitor, this, target, options, predicate);
            _visitor.AddLinkCandidate(candidate);
        }

        #endregion // LinkCandidate

        #region InternalSource

        ISourceBlock<T> ISourceHook<T>.InternalSource
        {
            get { return Source; }
        }

        #endregion // InternalSource
    }
}
