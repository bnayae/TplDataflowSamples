//#region Using

//using Bnaya.Samples;
//using Caliburn.Micro;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reactive.Concurrency;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading.Tasks.Dataflow;
//using Bnaya.Samples.Hooks;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Reactive;
//using System.Threading;
//using System.Reactive.Disposables;
//using System.Reactive.Subjects;
//using System.Windows;
//using System.Windows.Media;

//#endregion // Using

//namespace Bnaya.Samples.MVVM
//{
//    public class NonGreedyJoinScenario : ScenarioBase
//    {
//        private static readonly DataflowLinkOptions LINK_OPTIONS = new DataflowLinkOptions { PropagateCompletion = true };
//        private static readonly ExecutionDataflowBlockOptions BLOCK_OPTIONS = new ExecutionDataflowBlockOptions { BoundedCapacity = -1 };

//        #region Private / Protected Fields

//        private IPropogateHook<int, int> _blockFull;
//        private IPropogateHook<int, int> _blockEmpty;
//        private ITargetHook<int> _blockTarget;

//        private int _counter = 0;

//        private readonly AutoResetEvent _sync = new AutoResetEvent(false);

//        #endregion // Private / Protected Fields

//        #region Ctor

//        public NonGreedyJoinScenario()
//        {
//            Init();
//        }

//        #endregion // Ctor

//        #region Init

//        public void Init()
//        {
//            JoinBlock<
//            Data = new DataflowVisitor();

//            _blockFull = new BufferBlock<int>(BLOCK_OPTIONS)
//                .Hook("Full", Data, 10, 10, Colors.SteelBlue);

//            _blockEmpty = new BufferBlock<int>(BLOCK_OPTIONS)
//                .Hook("Empty", Data, 10, 450, Color.FromRgb(0x3D,0x04,0x60));

//            _blockTarget = new ActionBlock<int>(i =>
//            {
//                ReflectBlockExtensions.ProcessItem(i, _sync, _blockTarget.BlockInfo);
//            }, BLOCK_OPTIONS)
//                .Hook("Target", Data, 400, 200);

//            _blockEmpty.AddCommand("Offer Empty", OfferEmptyCommand);
//            _blockFull.AddCommand("Offer Full", OfferFullCommand);
//            _blockTarget.AddCommand("Release One", ReleaseSingleProcessingTaskCommand);

//            //_toolbar.DataContext = this;
//        }

//        #endregion // Init

//        #region Title

//        public override string Title { get { return "Consume To Accept"; } }

//        #endregion // Title

//        #region Order

//        public override double Order { get { return 500; } }

//        #endregion // Order

//        #region Toolbar

//        private FrameworkElement _toolbar = null;

//        public override FrameworkElement Toolbar
//        {
//            get { return _toolbar; }
//        }

//        #endregion // Toolbar

//        #region Commands

//        #region OfferEmptyCommand

//        public void OfferEmptyCommand()
//        {
//            int item = Interlocked.Increment(ref _counter);
//            //_bufferBlock1.Post(item);
//            var header = new DataflowMessageHeader(item);
//            var result = (_blockTarget as ITargetBlock<int>).OfferMessage(header, -1, _blockEmpty, true);
//        }

//        #endregion // OfferEmptyCommand

//        #region OfferFullCommand

//        public async void OfferFullCommand()
//        {
//            int item = Interlocked.Increment(ref _counter);
//            _blockFull.Post(item);
//            await Task.Delay(500);
//            //_bufferBlock1.Post(item);

//            long id = (long)_blockFull.BlockInfo.Properties.First(m => m.Name == "Next Header Id").Value;
//            var header = new DataflowMessageHeader(id);
//            var result = (_blockTarget as ITargetBlock<int>).OfferMessage(header, -1, _blockFull, true);
//        }

//        #endregion // OfferFullCommand

//        #region ReleaseSingleProcessingTaskCommand

//        public void ReleaseSingleProcessingTaskCommand()
//        {
//            _sync.Set();
//        }

//        #endregion // ReleaseSingleProcessingTaskCommand

//        #endregion // Commands
//    }
//}
