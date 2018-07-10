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
    public class BoundedCapacityBufferScenario : ScenarioBase
    {
        private static readonly DataflowLinkOptions LINK_OPTIONS = new DataflowLinkOptions { PropagateCompletion = true };
        private static readonly ExecutionDataflowBlockOptions BLOCK_OPTIONS = new ExecutionDataflowBlockOptions { BoundedCapacity = 3 };

        #region Private / Protected Fields

        private IPropogateHook<int, int> _block1;
        private IPropogateHook<int, int> _block2;
        private ITargetHook<int> _block3;

        private int _counter = 0;

        private readonly AutoResetEvent _sync = new AutoResetEvent(false);

        #endregion // Private / Protected Fields

        #region Ctor

        public BoundedCapacityBufferScenario()
        {
            Init();
        }

        #endregion // Ctor

        #region Init

        public void Init()
        {
            if (_block1 != null)
                _block1.Complete();

            Data = new DataflowVisitor();

            _block1 = new BufferBlock<int>(BLOCK_OPTIONS)
                .Hook("Buffer", Data, 10, 10, Colors.SteelBlue);
            _block1.AddCommand("Add Item", PostCommand);

            _block2 = new BufferBlock<int>(BLOCK_OPTIONS)
                .Hook("Buffer", Data, 250, 300, Color.FromRgb(0x3D,0x04,0x60));

            _block3 = new ActionBlock<int>(i =>
            {
                ReflectBlockExtensions.ProcessItem(i, _sync, _block3.BlockInfo);
            }, BLOCK_OPTIONS)
                .Hook("Action", Data, 700, 400);
            _block3.AddCommand("Release One", ReleaseSingleProcessingTaskCommand);

            _block1.LinkCandidate(_block2, LINK_OPTIONS);
            _block2.LinkCandidate(_block3, LINK_OPTIONS);

            //_toolbar.DataContext = this;
        }


        #endregion // Init

        #region Title

        public override string Title { get { return "Bounded Capacity Buffer"; } }

        #endregion // Title

        #region Order

        public override double Order { get { return 300; } }

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
            var result = (_block1 as ITargetBlock<int>).OfferMessage(header, item, null, false);
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
