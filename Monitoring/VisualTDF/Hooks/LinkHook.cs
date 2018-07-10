#region Using

using Bnaya.Samples.Hooks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

#endregion // Using

namespace Bnaya.Samples.Hooks
{
    [DebuggerDisplay("{LinkInfo}")]
    internal class LinkHook<T>: IPropagatorBlock<T, T>, IDisposable, ILinkHook
    {
        private ITargetBlock<T> _target;
        private readonly string _targetName = string.Empty;
        private readonly BlockInformation _targetInfo = null;
        private ISourceHook<T> _source;
        private IDisposable _disposal;
        private readonly  DataflowVisitor _visitor;
        private readonly LinkCandidateInformation<T> _candidate; 

        #region Ctor

        public LinkHook(
            ISourceHook<T> source, 
            ITargetBlock<T> target, 
            DataflowLinkOptions linkOptions,
            DataflowVisitor visitor,
            Predicate<T> predicate = null,
            LinkCandidateInformation<T> candidate = null)
        {
            _target = target;
            var targetHook = target as ITargetHook<T>;
            if (targetHook != null)
            {
                _targetName = targetHook.BlockInfo.Name;
                _targetInfo = targetHook.BlockInfo;
            }
            _source = source;

            _candidate = candidate;
            if (candidate != null)
                LinkInfo = new LinkToInformation(candidate.Connector, linkOptions, UnLink);

            if (predicate == null)
                _disposal = _source.InternalSource.LinkTo(this, linkOptions);
            else
                _disposal = DataflowBlock.LinkTo(_source.InternalSource, this, linkOptions, predicate);


            _visitor = visitor;
        }

        #endregion // Ctor

        public LinkToInformation LinkInfo { get; private set; }

        #region UnLink

        public void UnLink()
        {
            _disposal.Dispose();
            _visitor.RemoveLink(LinkInfo);
            _visitor.AddLinkCandidate<T>(_candidate);
        }

        #endregion // UnLink

        #region OfferMessage (hook)

        /// <summary>
        /// Offers the message (hook the link's messaging).
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
                _target.OfferMessage(messageHeader, messageValue, this, consumeToAccept);

            LinkInfo.PushOfferingCounter.Increment(result);
            var msg = new OfferMessageTrace(messageHeader.Id, messageValue, consumeToAccept, result, _source.BlockInfo.Name, _targetName);

            LinkInfo.PushOffering.Add(msg);
            if (_targetInfo != null)
                _targetInfo.PushOffering.Add(msg);
            return result;
        }

        #endregion // OfferMessage (hook)

        #region LinkTo (do nothing)

        /// <summary>
        /// Links to (the ctor do most of the work).
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="linkOptions">The link options.</param>
        /// <returns></returns>
        public IDisposable LinkTo(ITargetBlock<T> target, DataflowLinkOptions linkOptions)
        {
            throw new NotImplementedException();
        }

        #endregion // LinkTo (do nothing)

        #region ConsumeMessage (hook)

        /// <summary>
        /// Consumes the message (hook the link's messaging).
        /// </summary>
        /// <param name="messageHeader">The message header.</param>
        /// <param name="target">The target.</param>
        /// <param name="messageConsumed">if set to <c>true</c> [message consumed].</param>
        /// <returns></returns>
        public T ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target, out bool messageConsumed)
        {
            T result = _source.ConsumeMessage(messageHeader, target, out messageConsumed);

            LinkInfo.PoolRequestCounters.Increment(messageConsumed);

            var msg = new ConsumeTrace(messageHeader.Id, messageConsumed, _source.BlockInfo.Name, _targetName, result);

            LinkInfo.PoolRequest.Add(msg);
            _source.BlockInfo.PoolRequest.Add(msg);
            return result;
        }

        #endregion // ConsumeMessage (hook)

        #region ReleaseReservation (hook)

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            _source.ReleaseReservation(messageHeader, this);
            LinkInfo.PoolRequestCounters.IncrementReleaseReservation();
        }

        #endregion // ReleaseReservation (hook)

        #region ReserveMessage (hook)

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<T> target)
        {
            bool result = _source.ReserveMessage(messageHeader, this);
            LinkInfo.PoolRequestCounters.IncrementReserve();
            return result;
        }

        #endregion // ReserveMessage (hook)

        #region IDataflowBlock Members

        public async void Complete()
        {
            if (_target != null)
                _target.Complete();
            //if (_source != null)
            //    _source.Complete();
            if (_target != null)
                await _target.Completion;
            //if (_source != null)
            //    await _source.Completion;
            Dispose();
        }

        public async void Fault(Exception exception)
        {
            if (_target != null)
                _target.Fault(exception);
            //if (_source != null)
            //    _source.Fault(exception);
            if (_target != null)
                await _target.Completion;
            //if (_source != null)
            //    await _source.Completion;
            Dispose();
        }

        public Task Completion
        {
            get { return _target.Completion; }
        }

        #endregion // IDataflowBlock Members

        #region Equals

        public override bool Equals(object obj)
        {
            return (this == obj as LinkHook<T>);
        }

        #endregion // Equals

        #region GetHashCode

        public override int GetHashCode()
        {
            return this.LinkInfo.Connector.GetHashCode();
        }

        #endregion // GetHashCode

        #region operator ==

        public static bool operator ==(LinkHook<T> x, LinkHook<T> y)
        {
            if (y == (object)null)
                return false;
            return x.LinkInfo.Connector == y.LinkInfo.Connector;
        }

        #endregion // operator ==

        #region operator !=

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(LinkHook<T> x, LinkHook<T> y)
        {
            return !(x == y);
        }

        #endregion // operator !=

        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _disposal.Dispose();
                _source = null;
                _target = null;
                _visitor.RemoveLink(LinkInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion // Dispose
    }
}
