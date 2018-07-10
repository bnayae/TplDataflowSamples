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
    public class BoundedCapacityStarvationTransformScenario : ScenarioBase
    {
        private static readonly DataflowLinkOptions LINK_OPTIONS = new DataflowLinkOptions { PropagateCompletion = true };
        private static readonly ExecutionDataflowBlockOptions BLOCK_OPTIONS = new ExecutionDataflowBlockOptions { BoundedCapacity = 5 };

        #region Private / Protected Fields

        private IPropogateHook<int, int> _sourceBlock;
        private IPropogateHook<int, string> _block;
        private ITargetHook<string> _nullTarget;

        private IDisposable _unlinkGarbage;

        private int _counter = 0;

        private readonly AutoResetEvent _sync = new AutoResetEvent(false);

        #endregion // Private / Protected Fields

        #region Ctor

        public BoundedCapacityStarvationTransformScenario()
        {
            Init();
        }

        #endregion // Ctor

        #region Init

        public void Init()
        {
            if (_sourceBlock != null)
                _sourceBlock.Complete();

            Data = new DataflowVisitor();

            _sourceBlock = new BufferBlock<int>()
                .Hook("Buffer", Data, 10, 300, Colors.SteelBlue);
            _sourceBlock.AddCommand("Add Item", PostCommand);

            // TODO: know complete processing counter
            _block = new TransformBlock<int, string>(i =>
            {
                ReflectBlockExtensions.ProcessItem(i, _sync, _block.BlockInfo);
                return new string('*', i);
            }, BLOCK_OPTIONS)
                .Hook("Transform", Data, 500, 10);
            _block.AddCommand("Release One", ReleaseSingleProcessingTaskCommand);

            var nullTarget = DataflowBlock.NullTarget<int>()
                .Hook("NullTarget", Data, 500, 500);

            _nullTarget = DataflowBlock.NullTarget<string>()
                .Hook("NullTarget", Data, 900, 200);

            _sourceBlock.LinkTo(_block, LINK_OPTIONS);
            _unlinkGarbage = _sourceBlock.LinkTo(nullTarget, LINK_OPTIONS);

            _block.LinkCandidate(_nullTarget, LINK_OPTIONS);

            //_toolbar.DataContext = this;
        }

        #endregion // Init

        #region Title

        public override string Title { get { return "Bounded Capacity Starvation Transform"; } }

        #endregion // Title

        #region Order

        public override double Order { get { return 110; } }

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

        public void ReleaseSingleProcessingTaskCommand()
        {
            _sync.Set();
        }

        #endregion // ReleaseSingleProcessingTaskCommand

        #endregion // Commands
    }
}
