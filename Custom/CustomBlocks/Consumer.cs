#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Collections.Concurrent;

#endregion // Using

namespace TDF.Samples
{
    #region Documentation
    /// <summary>
    /// Custom simple consumer implementation
    /// </summary>
    #endregion // Documentation
    public class Consumer: ITargetBlock<long>
    {
        #region Private / Protected Fields

        private TaskCompletionSource<object> _blockTask = new TaskCompletionSource<object>();

        #endregion // Private / Protected Fields

        #region ITargetBlock<long> Members

        #region Documentation
        /// <summary>
        /// Consume message
        /// </summary>
        /// <param name="messageHeader">
        /// The DataflowMessageHeader of the message being consumed.
        /// </param>
        /// <param name="messageValue"></param>
        /// <param name="source"></param>
        /// <param name="consumeToAccept"></param>
        /// <returns>
        /// Accepted: accept the message and take ownership.
        /// Declined:
        /// Postponed: postponed the message for potential consumption at a later time.
        ///            The target may consume the message later by using the source ConsumeMessage.
        /// NotAvailable: fail to consume the message (the message was no longer available)
        /// DecliningPermanently: declined the message and all future messages sent by the source. 
        /// </returns>
        #endregion // Documentation
        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader, 
            long messageValue, 
            ISourceBlock<long> source, 
            bool consumeToAccept)
        {
            Console.WriteLine(messageValue);
            return DataflowMessageStatus.Accepted;
        }

        #endregion // ITargetBlock<long> Members

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
            _blockTask.SetResult(null);
        }

        #endregion // Complete

        #region Completion

        #region Documentation
        /// <summary>
        /// Signals to the System.Threading.Tasks.Dataflow.IDataflowBlock that it should
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
        /// Causes the System.Threading.Tasks.Dataflow.IDataflowBlock to complete in
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
            _blockTask.SetException(exception);
        }

        #endregion // Fault

        #endregion // IDataflowBlock Members
    }
}
