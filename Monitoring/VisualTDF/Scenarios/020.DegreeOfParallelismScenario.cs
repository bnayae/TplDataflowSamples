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
    public class DegreeOfParallelismScenario : ScenarioBase
    {
        private static readonly DataflowLinkOptions LINK_OPTIONS = new DataflowLinkOptions { PropagateCompletion = true };
        private static readonly ExecutionDataflowBlockOptions BLOCK_OPTIONS = 
            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3};

        #region Private / Protected Fields

        private IPropogateHook<int, int> _source;
        private ITargetHook<int> _target;

        private IDisposable _unlinkGarbage;

        private int _counter = 0;

        private readonly AutoResetEvent _sync = new AutoResetEvent(false);

        #endregion // Private / Protected Fields

        #region Ctor

        public DegreeOfParallelismScenario()
        {
            Init();
        }

        #endregion // Ctor

        #region Init

        public void Init()
        {
            if (_source != null)
                _source.Complete();

            Data = new DataflowVisitor();

            _source = new BufferBlock<int>(BLOCK_OPTIONS)
                .Hook("Buffer", Data, 100, 10, Colors.SteelBlue);
            _source.AddCommand("Add Item", PostCommand);

            _target = new ActionBlock<int>(i =>
            {
                ReflectBlockExtensions.ProcessItem(i, _sync, _target.BlockInfo);
            }, BLOCK_OPTIONS)
                .Hook("Action", Data, 600, 200);
            _target.AddCommand("Release One", ReleaseSingleProcessingTaskCommand);

            _source.LinkCandidate(_target, LINK_OPTIONS);

            //_toolbar.DataContext = this;
        }


        #endregion // Init

        #region Title

        public override string Title { get { return "MaxDegree Of Parallelism"; } }

        #endregion // Title

        #region Order

        public override double Order { get { return 20; } }

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
            var header = new DataflowMessageHeader(item);
            var result = (_source as ITargetBlock<int>).OfferMessage(header, item, null, false);
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
    