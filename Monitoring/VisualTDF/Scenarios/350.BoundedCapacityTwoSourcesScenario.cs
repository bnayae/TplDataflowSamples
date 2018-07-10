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
    public class BoundedCapacityTwoSourcesScenario : ScenarioBase
    {
        private static readonly DataflowLinkOptions LINK_OPTIONS = new DataflowLinkOptions { PropagateCompletion = true };
        private static readonly ExecutionDataflowBlockOptions BLOCK_OPTIONS = new ExecutionDataflowBlockOptions { BoundedCapacity = 3 };

        #region Private / Protected Fields

        private IPropogateHook<int, int> _blockA;
        private IPropogateHook<int, int> _blockB;
        private ITargetHook<int> _block3;

        private IDisposable _unlinkGarbage;

        private int _counter = 0;

        private readonly AutoResetEvent _sync = new AutoResetEvent(false);

        #endregion // Private / Protected Fields

        #region Ctor

        public BoundedCapacityTwoSourcesScenario()
        {
            Init();
        }

        #endregion // Ctor

        #region Init

        public void Init()
        {
            if (_blockA != null)
                _blockA.Complete();

            Data = new DataflowVisitor();

            _blockA = new BufferBlock<int>(BLOCK_OPTIONS)
                .Hook("Buffer", Data, 10, 10, Colors.SteelBlue);
            _blockA.AddCommand("Add Item", PostACommand);

            _blockB = new BufferBlock<int>(BLOCK_OPTIONS)
                .Hook("Buffer", Data, 10, 400, Color.FromRgb(0x3D,0x04,0x60));
            _blockB.AddCommand("Add Item", PostBCommand);

            _block3 = new ActionBlock<int>(i =>
            {
                ReflectBlockExtensions.ProcessItem(i, _sync, _block3.BlockInfo);
            }, BLOCK_OPTIONS)
                .Hook("Action", Data, 600, 200);
            _block3.AddCommand("Release One", ReleaseSingleProcessingTaskCommand);

            _blockA.LinkCandidate(_block3, LINK_OPTIONS);
            _blockB.LinkCandidate(_block3, LINK_OPTIONS);

            //_toolbar.DataContext = this;
        }


        #endregion // Init

        #region Title

        public override string Title { get { return "Bounded Capacity Two Sources"; } }

        #endregion // Title

        #region Order

        public override double Order { get { return 350; } }

        #endregion // Order

        #region Toolbar

        private FrameworkElement _toolbar = null;

        public override FrameworkElement Toolbar
        {
            get { return _toolbar; }
        }

        #endregion // Toolbar

        #region Commands

        #region PostACommand

        public void PostACommand()
        {
            int item = Interlocked.Increment(ref _counter);
            var header = new DataflowMessageHeader(item);
            var result = (_blockA as ITargetBlock<int>).OfferMessage(header, item, null, false);
        }

        #endregion // PostACommand

        #region PostBCommand

        public void PostBCommand()
        {
            int item = Interlocked.Increment(ref _counter);
            var header = new DataflowMessageHeader(item);
            var result = (_blockB as ITargetBlock<int>).OfferMessage(header, item, null, false);
        }

        #endregion // PostBCommand

        #region ReleaseSingleProcessingTaskCommand

        public void ReleaseSingleProcessingTaskCommand()
        {
            _sync.Set();
        }

        #endregion // ReleaseSingleProcessingTaskCommand

        #endregion // Commands
    }
}
    