using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class ConsumeTrace
    {
        public ConsumeTrace(
            long messageId,
            bool messageConsumed,
            string source, 
            string target, 
            object response)
	    {
            MessageId = messageId;
            Source = source;
            Target = target;
            MessageConsumed = messageConsumed;
            Response = response == null ? null : response.ToString();
	    }

        public long MessageId { get; private set; }
        public string Source { get; private set; }
        public string Target { get; private set; }
        public bool MessageConsumed { get; private set; }
        public string Response { get; set; }
    }
}
