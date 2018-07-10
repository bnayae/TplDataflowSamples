using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class OfferMessageTrace
    {
        public OfferMessageTrace(
            long messageId,
            object value,
            bool consumeToAccept,
            DataflowMessageStatus response,
            string source = "",
            string target = "")
	    {
            MessageId = messageId;
            Value = (value ?? string.Empty).ToString();
            Source = source;
            Target = target;
            ConsumeToAccept = consumeToAccept;
            Response = response;
	    }

        public long MessageId { get; private set; }
        public string Value { get; private set; }
        public string Source { get; private set; }
        public string Target { get; private set; }
        public bool ConsumeToAccept { get; private set; }
        public DataflowMessageStatus Response { get; set; }
    }
}
