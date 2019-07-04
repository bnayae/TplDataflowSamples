using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageMnifFlowUI
{

    public class ToUI
    {
        private readonly ActionBlock<(byte[] data, string topic, int index)> _worker;
        private readonly IProgress<byte[]> _notifier;

        public ToUI(IProgress<byte[]> notifier)
        {
            _notifier = notifier;
            _worker = new ActionBlock<(byte[] data, string topic, int index)>(DoSaveAsync);
        }

        public ITargetBlock<(byte[] data, string topic, int index)> Target => _worker;

        private void DoSaveAsync((byte[] input, string topic, int index) data)
        {
            _notifier.Report(data.input);
        }
    }

}
