using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class CustomTarget: ITargetBlock<int>
    {
        private Func<bool> _canAccept;
        private string _name;
        public CustomTarget(string name, Func<bool> canAccept)
        {
            _name = name;
            _canAccept = canAccept;
        }

        public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader, int messageValue, ISourceBlock<int> source, bool consumeToAccept)
        {
            if (!_canAccept())
            {
                if(source != null)
                    Consume(source, messageHeader);
                return DataflowMessageStatus.Postponed;
            }

            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\tOffered End: [{0}] Id = {1}", _name, messageHeader.Id);
                Console.ResetColor();
            }
            return DataflowMessageStatus.Accepted;
        }

        private async void Consume(ISourceBlock<int> source, DataflowMessageHeader messageHeader)
        {
            await Task.Delay(3000);
            bool succeed;
            int i = source.ConsumeMessage(messageHeader, this, out succeed);
            lock (Program._sync)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\tConsumed: {0}: Id = {1}, succeed = {2}", _name, messageHeader.Id, succeed);
                Console.ResetColor();
            }
        }

        public void Complete()
        {
            throw new NotImplementedException();
        }

        public Task Completion
        {
            get { throw new NotImplementedException(); }
        }

        public void Fault(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
