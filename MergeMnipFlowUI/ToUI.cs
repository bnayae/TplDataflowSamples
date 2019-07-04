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

namespace Bnaya.Samples
{
    public class ToUI
    {
        private readonly ActionBlock<ImageState> _worker;
        private readonly IProgress<ImmutableArray<byte>> _notifier;

        public ToUI(IProgress<ImmutableArray<byte>> notifier)
        {
            _notifier = notifier;
            _worker = new ActionBlock<ImageState>(DoSaveAsync);
        }

        public ITargetBlock<ImageState> Target => _worker;

        private void DoSaveAsync(ImageState data)
        {
            _notifier.Report(data.Data);
        }
    }

}
