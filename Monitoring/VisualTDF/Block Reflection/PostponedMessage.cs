using Bnaya.Samples.Hooks;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

namespace Bnaya.Samples
{
    public class PostponedMessage 
    {
        public PostponedMessage(IDataflowBlock block, DataflowMessageHeader header)
        {
            MessageId = header.Id;
            var linkHook = block as ILinkHook;
            if (linkHook != null)
                Link = linkHook.LinkInfo;
        }
        public LinkToInformation Link { get; private set; }
        public long MessageId { get; private set; }
    }
}
