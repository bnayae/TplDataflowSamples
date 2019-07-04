using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private readonly ActionBlock<(ImmutableArray<byte> data, string topic, int index)> _worker;
        private readonly IProgress<ImmutableArray<byte>> _notifier;

        public ToUI(IProgress<ImmutableArray<byte>> notifier)
        {
            _notifier = notifier;
            _worker = new ActionBlock<(ImmutableArray<byte> data, string topic, int index)>(DoSaveAsync);
        }

        public ITargetBlock<(ImmutableArray<byte> data, string topic, int index)> Target => _worker;

        private void DoSaveAsync((ImmutableArray<byte> input, string topic, int index) data)
        {
            _notifier.Report(data.input);
        }
    }

}
