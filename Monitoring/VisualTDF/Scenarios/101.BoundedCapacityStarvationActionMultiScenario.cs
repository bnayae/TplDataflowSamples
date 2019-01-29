#region Using

using Bnaya.Samples;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Bnaya.Samples.Hooks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reactive;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples.MVVM
{
    public class BoundedCapacityStarvationActionMultiScenario : ScenarioBase 
    {
        private static readonly DataflowLinkOptions LINK_OPTIONS = new DataflowLinkOptions { PropagateCompletion = true };
        private static readonly ExecutionDataflowBlockOptions BLOCK_OPTIONS = new ExecutionDataflowBlockOptions { BoundedCapacity = 3 };

        #region Private / Protected Fields

        private IPropogateHook<int, int> _sourceBlock;
        private ITargetHook<int> _targetBlockA;
        private ITargetHook<int> _targetBlockB;

        private IDisposable _unlinkGarbage;

        private int _counter = 0;

        private readonly AutoResetEvent _syncA = new AutoResetEvent(false);
        private readonly AutoResetEvent _syncB = new AutoResetEvent(false);

        #endregion // Private / Protected Fields

        #region Ctor

        public BoundedCapacityStarvationActionMultiScenario()
        {
            Init();
        }

        #endregion // Ctor

        #region Init

        private void Init()
        {
            if (_sourceBlock != null)
                _sourceBlock.Complete();

            Data = new DataflowVisitor();

            _sourceBlock = new BufferBlock<int>()
                .Hook("Buffer", Data, 50, 300, Colors.SteelBlue);
            _sourceBlock.AddCommand("Add Item", PostCommand);

            _targetBlockA = new ActionBlock<int>(i =>
            {
                ReflectBlockExtensions.ProcessItem(i, _syncA, _targetBlockA.BlockInfo); 
            }, BLOCK_OPTIONS)
                .Hook("Action A", Data, 800, 10);
            _targetBlockA.AddCommand("Release One", () => ReleaseSingleProcessingTaskCommand(_syncA));

            _targetBlockB = new ActionBlock<int>(i =>
            {
                ReflectBlockExtensions.ProcessItem(i, _syncB, _targetBlockB.BlockInfo); 
            }, BLOCK_OPTIONS)
                .Hook("Action B", Data, 800, 200);
            _targetBlockB.AddCommand("Release One", () => ReleaseSingleProcessingTaskCommand(_syncB));

            var nullTarget = DataflowBlock.NullTarget<int>()
                .Hook("NullTarget", Data, 500, 500);

            _sourceBlock.LinkTo(_targetBlockA, LINK_OPTIONS);
            _sourceBlock.LinkTo(_targetBlockB, LINK_OPTIONS);
            _unlinkGarbage = _sourceBlock.LinkTo(nullTarget, LINK_OPTIONS);

            //_toolbar.DataContext = this;
        }

        #endregion // Init

        #region Title

        public override string Title { get { return "Bounded Capacity Starvation Multi Action"; } }

        #endregion // Title

        #region Order

        public override double Order { get { return 101; } }

        #endregion // Order

        #region Toolbar

        private FrameworkElement _toolbar = null;

        public override FrameworkElement Toolbar
        {
            get { return _toolbar; }
        }

        #endregion // Toolbar

        #region Commands

        #region PostCommand

        public void PostCommand()
        {
            int item = Interlocked.Increment(ref _counter);
            //_bufferBlock1.Post(item);
            var header = new DataflowMessageHeader(item);
            var result = (_sourceBlock as ITargetBlock<int>).OfferMessage(header, item, null, false);
        }

        #endregion // PostCommand

        #region ReleaseSingleProcessingTaskCommand

        public void ReleaseSingleProcessingTaskCommand(AutoResetEvent sync)
        {
            sync.Set();
        }

        #endregion // ReleaseSingleProcessingTaskCommand

        #endregion // Commands
    }
}
