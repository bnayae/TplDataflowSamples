#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Disposables;

#endregion // Using

namespace TDF.Samples
{
    #region Documentation
    /// <summary>
    /// Custom simple provider implementation
    /// </summary>
    #endregion // Documentation
    public class Producer: ISourceBlock<long>
    {
        #region Private / Protected Fields

        private readonly ConcurrentDictionary<ITargetBlock<long>, object> _targets =
            new ConcurrentDictionary<ITargetBlock<long>, object>();
        private Timer _tmr;
        private long _counter = 0;

        #region Support for cancellation

        private TaskCompletionSource<object> _blockTask = new TaskCompletionSource<object>();
        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        #endregion // Support for cancellation

        #endregion // Private / Protected Fields

        #region Constructors

        public Producer()
        {
            _tmr = new Timer(OnTimerCallback, null, 0, 500);
        }

        #endregion // Constructors

        #region OnTimerCallback

        #region Documentation
        /// <summary>
        /// Produce values
        /// </summary>
        /// <param name="state"></param>
        #endregion // Documentation
        private void OnTimerCallback(object state)
        {
            long counter = _counter;
            Parallel.ForEach(_targets.Keys,
                target =>
                {
                    var header = new DataflowMessageHeader(counter);
                    DataflowMessageStatus result =
                        target.OfferMessage(header, counter, this, false);

                    #region Result Validation

                    switch (result)
                    {
                        case DataflowMessageStatus.Accepted:
                            Trace.Write("Accepted");
                            break;
                        case DataflowMessageStatus.Declined:
                            Trace.Write("Declined");
                            break;
                        case DataflowMessageStatus.DecliningPermanently:
                            object tmp;
                            _targets.TryRemove(target, out tmp);
                            Trace.Write("DecliningPermanently");
                            break;
                        case DataflowMessageStatus.NotAvailable:
                            Trace.Write("NotAvailable");
                            break;
                        case DataflowMessageStatus.Postponed:
                            Trace.Write("Postponed"); 
                            break;
                    }

                    #endregion // Result Validation
                });

                Interlocked.Increment(ref _counter);
        }

        #endregion // OnTimerCallback

        #region ISourceBlock<long> Members

        #region LinkTo

        #region Documentation
        /// <summary>
        /// Links the ISourceBlock<TOutput> to the specified
        /// ITargetBlock<TInput>.
        /// </summary>
        /// <param name="target">
        /// The ITargetBlock<TInput> to which to connect
        /// this source</param>
        /// <param name="linkOptions">
        /// </param>
        /// <returns>
        /// An IDisposable that, upon calling Dispose, will unlink the source from the target.
        /// </returns>
        /// <exceptions>
        /// System.ArgumentNullException: The target is null
        /// </exceptions>
        #endregion // Documentation
        public IDisposable LinkTo(ITargetBlock<long> target, DataflowLinkOptions linkOptions)
        {
            _targets.AddOrUpdate(target, 0, (key, value) => 0);

            var linkDisposal = new CancellationDisposable();
            linkDisposal.Token.Register(() => 
                {
                    object item;
                    _targets.TryRemove(target, out item);
                });

            return linkDisposal;
        }

        #endregion // LinkTo

        #region ConsumeMessage

        #region Documentation
        /// <summary>
        /// Called by a linked ITargetBlock<TInput> to
        /// accept and consume a DataflowMessageHeader
        /// previously offered by this ISourceBlock<TOutput>.
        /// </summary>
        /// <param name="messageHeader">
        /// The DataflowMessageHeader of the message being consumed.
        /// </param>
        /// <param name="target">
        /// The ITargetBlock<TInput> consuming the message
        /// </param>
        /// <param name="messageConsumed">
        /// True if the message was successfully consumed. False otherwise.
        /// </param>
        /// <returns>
        /// The value of the consumed message. This may correspond to a different DataflowMessageHeader
        /// instance than was previously reserved and passed as the messageHeader to
        /// ConsumeMessage. The consuming ITargetBlock<TInput>
        /// must use the returned value instead of the value passed as messageValue through
        /// OfferMessage.
        /// If the message requested is not available, the return value will be null.
        /// </returns>
        /// <exception>
        /// System.ArgumentException: The messageHeader is not valid.
        /// </exception>
        /// <exception>
        /// System.ArgumentNullException: The target is null.
        /// </exception>
        /// <remarks>
        /// Only ITargetBlock<TInput> instances linked
        /// to this ISourceBlock<TOutput> instance may
        /// use ConsumeMessage, and it must only be used to reserve DataflowMessageHeader
        /// instances previously offered by this source to the target.
        /// </remarks>
        #endregion // Documentation
        public long ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<long> target, out bool messageConsumed)
        {
            throw new NotImplementedException();
        }

        #endregion // ConsumeMessage

        #region ReleaseReservation

        #region Documentation
        /// <summary>
        /// Called by a linked ITargetBlock<TInput> to
        /// release a previously reserved DataflowMessageHeader
        /// by this ISourceBlock<TOutput>.
        /// </summary>
        /// <param name="messageHeader">
        /// The DataflowMessageHeader of the message being consumed.
        /// </param>
        /// <param name="target">
        /// The ITargetBlock<TInput> releasing the message it previously reserved.
        /// </param>
        /// <exception>
        /// System.ArgumentException: The messageHeader is not valid.
        /// </exception>
        /// <exception>
        /// System.ArgumentNullException: The target is null.
        /// </exception>
        /// <exception>
        /// System.InvalidOperationException: The target did not have the message reserved.
        /// </exception>
        /// <remarks>
        /// Only ITargetBlock<TInput> instances linked
        /// to this ISourceBlock<TOutput> instance may
        /// use ReleaseMessage, and it must only be used to release DataflowMessageHeader
        /// instances previously and successfully reserved by the target.
        /// </remarks>
        #endregion // Documentation
        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<long> target)
        {
            throw new NotImplementedException();
        }

        #endregion // ReleaseReservation

        #region ReserveMessage

        #region Documentation
        /// <summary>
        /// Called by a linked ITargetBlock<TInput> to
        /// reserve a previously offered DataflowMessageHeader
        /// by this ISourceBlock<TOutput>.
        /// </summary>
        /// <param name="messageHeader">
        /// The DataflowMessageHeader of the message being consumed.
        /// </param>
        /// <param name="target">
        /// The ITargetBlock<TInput> reserving the message.
        /// </param>
        /// <returns>
        /// true if the message was successfully reserved; otherwise, false.
        /// </returns>
        /// <exception>
        /// System.ArgumentException: The messageHeader is not valid.
        /// </exception>
        /// <exception>
        /// System.ArgumentNullException: The target is null.
        /// </exception>
        /// <remarks>
        /// Only ITargetBlock<TInput> instances linked
        /// to this ISourceBlock<TOutput> instance may
        /// use ReserveMessage, and it must only be used to reserve DataflowMessageHeader
        /// instances previously offered by this source to the target.
        /// If true is returned, the ITargetBlock<TInput>
        /// must subsequently call either ConsumeMessage or ReserveMessage for this message.
        ///  Failure to do so may result in the source being unable to propagate any
        /// further messages to this or other targets.
        /// ReserveMessage must not be called while the target is holding any internal
        /// locks. Doing so will violate the lock hierarchy necessary to avoid deadlocks
        /// in a dataflow network.
        /// </remarks>
        #endregion // Documentation
        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<long> target)
        {
            throw new NotImplementedException();
        }

        #endregion // ReserveMessage

        #endregion // ISourceBlock<long> Members

        #region IDataflowBlock Members

        #region Complete

        #region Documentation
        /// <summary>
        /// Gets a System.Threading.Tasks.Task that represents the asynchronous operation
        /// and completion of the dataflow block.
        /// </summary>
        /// <remarks>
        /// A dataflow block is considered completed when it is not currently processing
        /// a message and when it has guaranteed that it will not process any more messages.
        /// The returned System.Threading.Tasks.Task will transition to a completed state
        /// when the associated block has completed. It will transition to the System.Threading.Tasks.TaskStatus
        /// state when the block completes its processing successfully according to the
        /// dataflow block’s defined semantics, it will transition to the System.Threading.Tasks.TaskStatus
        /// state when the dataflow block has completed processing prematurely due to
        /// an unhandled exception, and it will transition to the System.Threading.Tasks.TaskStatus
        /// state when the dataflow block has completed processing prematurely due to
        /// receiving a cancellation request. If the task completes in the Faulted state,
        /// its Exception property will return an System.AggregateException containing
        /// the one or more exceptions that caused the block to fail.
        /// </remarks>
        #endregion // Documentation
        public void Complete()
        {
            _tmr.Dispose();
            _blockTask.SetResult(null);
        }

        #endregion // Complete

        #region Completion

        #region Documentation
        /// <summary>
        /// Signals to the IDataflowBlock that it should
        /// not accept nor produce any more messages nor consume any more postponed messages.
        /// </summary>
        /// <remarks>
        /// After Complete has been called on a dataflow block, that block will complete
        /// (such that its Completion task will enter a final state) after it's processed
        /// all previously available data. Complete will not block waiting for completion
        /// to occur, but rather will initiaite the request; to wait for completion to
        /// occur, the Completion task may be used.
        /// </remarks>
        #endregion // Documentation
        public Task Completion
        {
            get { return _blockTask.Task; }
        }

        #endregion // Completion

        #region Fault

        #region Documentation
        /// <summary>
        /// Causes the IDataflowBlock to complete in
        /// a System.Threading.Tasks.TaskStatus.Faulted state.
        /// </summary>
        /// <param name="exception">The System.Exception that caused the faulting</param>
        /// <remarks>
        /// After Fault has been called on a dataflow block, that block will complete
        /// (such that its Completion task will enter a final state). Faulting a block
        /// causes buffered messages (uprocessed input messages as well as unoffered
        /// output messages) to be lost.
        /// </remarks>
        #endregion // Documentation
        public void Fault(Exception exception)
        {
            _tmr.Dispose();
            _blockTask.SetException(exception);
        }

        #endregion // Fault

        #endregion // IDataflowBlock Members

    }
}
