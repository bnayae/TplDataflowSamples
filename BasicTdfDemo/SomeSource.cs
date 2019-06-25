using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BasicTdfDemo
{
    public class SomeSource : ISourceBlock<int>
    {
        private readonly object _gate = new object();
        private readonly List<ITargetBlock<int>> _linked = new List<ITargetBlock<int>>();
        private readonly ConcurrentQueue<int> _data = new ConcurrentQueue<int>();
        private readonly Timer _tmr;
        private bool _completing;
        private int _counter = -1;

        public SomeSource()
        {
            _tmr = new Timer(s => _data.Enqueue(Interlocked.Increment(ref _counter)), null, 100, 100);
            Task _ = Task.Run(Work);
        }

        private async Task Work()
        {
            long id = 0;
            do
            {
                while (_data.TryDequeue(out int i))
                {
                    id++;
                    lock (_gate)
                    {
                        for (int j = _linked.Count - 1; j >= 0; j--)
                        {
                            var target = _linked[j];

                            var header = new DataflowMessageHeader(id);
                            DataflowMessageStatus status =
                                    target.OfferMessage(header, i, this, false);
                            if (status == DataflowMessageStatus.Accepted)
                                break;
                            if (status == DataflowMessageStatus.DecliningPermanently)
                                _linked.RemoveAt(j);
                            Console.Write($" {status} ");

                        }
                    }
                }
                await Task.Delay(50).ConfigureAwait(false);
            } while (!_completing);
        }

        public IDisposable LinkTo(
            ITargetBlock<int> target,
            DataflowLinkOptions linkOptions)
        {
            lock (_gate)
            {
                _linked.Insert(0, target);
            }

            return new Unsubscribe(() =>
            {
                lock (_gate)
                {
                    _linked.Remove(target);
                }
            });
        }

        private class Unsubscribe : IDisposable
        {
            private readonly Action _onDispose;

            public Unsubscribe(Action onDispose)
            {
                _onDispose = onDispose;
            }
            public void Dispose() => _onDispose();
        }

        public void Complete()
        {
            _tmr.Dispose();
            _completing = true;
        }

        public void Fault(Exception exception)
        {
            throw new NotImplementedException();
        }

        public Task Completion => throw new NotImplementedException();



        public int ConsumeMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target, out bool messageConsumed)
        {
            throw new NotImplementedException();
        }

        public void ReleaseReservation(DataflowMessageHeader messageHeader, ITargetBlock<int> target)
        {
            throw new NotImplementedException();
        }

        public bool ReserveMessage(DataflowMessageHeader messageHeader, ITargetBlock<int> target)
        {
            throw new NotImplementedException();
        }
    }
}
