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
    internal class HookBase: IDataflowBlock, IDataflowHook
    {
        private const int DEFAULT_REFRESH_INTERVAL_SEC = 1;
        // generic way to trigger refresh (can be block delegate wrapper or Observable.Interval + scheduler)
        protected DataflowVisitor _visitor;
        protected IReflectBlock _reflectView;
        protected IDataflowBlock _block;

        #region Ctor

        public HookBase(
            string name, 
            IDataflowBlock block,
            DataflowVisitor visitor,
            Color? color = null)
        {
            #region Validation

            if (visitor == null)
                throw new ArgumentNullException("visitor cannot be null");
            if (block == null)
                throw new ArgumentNullException("_block cannot be null");

            #endregion // Validation

            _block = block;
            color = color ?? Colors.DarkGray;

            _visitor = visitor;
            BlockInfo = new BlockInformation(name,
                _block.IsImplements(typeof(ISourceBlock<>)), 
                _block.IsImplements(typeof(ITargetBlock<>)), 
                color.ToString());

            _visitor.AddBlock(BlockInfo);
            LifeTimeWatcher();
        }

        #endregion // Ctor

        public BlockInformation BlockInfo { get; private set; }

        #region AddCommand

        #region Overloads

        public void AddCommand(string title, Action execute)
        {
            AddCommand(title, execute, Color.FromRgb(170, 200, 240));
        }
        public void AddCommand(string title, Action execute, Color backColor)
        {
            AddCommand(title, execute, backColor, Colors.Black);
        }

        #endregion // Overloads

        public void AddCommand(string title, Action execute, Color backColor, Color foreColor)
        {
            var command = new GenericCommand(title, execute, backColor, foreColor);
            BlockInfo.Commands.Add(command);
        }

        #endregion // AddCommand

        #region LifeTimeWatcher

        private async void LifeTimeWatcher()
        {
            await _block.Completion;
            if (_block.Completion.IsCanceled)
                BlockInfo.State = BlockState.Canceled;
            else if (_block.Completion.IsFaulted)
                BlockInfo.State = BlockState.Faulted;
            else 
                BlockInfo.State = BlockState.Completed;

            _visitor.RemoveBlock(BlockInfo);
            _visitor = null;
        }

        #endregion // LifeTimeWatcher

        #region Complete

        public void Complete()
        {
            _block.Complete();
        }

        #endregion // Complete

        #region Fault

        /// <summary>
        /// Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.
        /// </summary>
        /// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
        public void Fault(Exception exception)
        {
            _block.Fault(exception);
        }

        #endregion // Fault

        #region Completion

        /// <summary>
        /// Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.
        /// </summary>
        /// <returns>The task.</returns>
        public Task Completion
        {
            get 
            {
                return _block.Completion;
            }
        }

        #endregion // Completion
    }
}
