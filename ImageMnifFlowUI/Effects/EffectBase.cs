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

    public abstract class EffectBase
    {
        private readonly TransformBlock<(ImmutableArray<byte> data, string topic, int index), (ImmutableArray<byte> data, string topic, int index)> _worker;

        public EffectBase()
        {
            _worker = new TransformBlock<(ImmutableArray<byte> data, string topic, int index), (ImmutableArray<byte> data, string topic, int index)>(DoEffectAsync);
        }

        public ISourceBlock<(ImmutableArray<byte> data, string topic, int index)> Source => _worker;
        public ITargetBlock<(ImmutableArray<byte> data, string topic, int index)> Target => _worker;

        protected virtual (ImmutableArray<byte> data, string topic, int index) DoEffectAsync((ImmutableArray<byte> input, string topic, int index) data)
        {
            (ImmutableArray<byte> input, _, _) = data;
            Image<Rgba32> image = Image.Load(input.ToBuilder().ToArray());
            image.Mutate(OnEffect);
            var output = new MemoryStream();
            image.SaveAsJpeg(output);
            return (ImmutableArray.CreateRange(output.ToArray()), data.topic, data.index);
        }

        protected abstract void OnEffect(IImageProcessingContext<Rgba32> context);
    }

}
