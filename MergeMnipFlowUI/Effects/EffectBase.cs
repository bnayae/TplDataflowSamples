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

    public abstract class EffectBase
    {
        private readonly TransformBlock<ImageState, ImageState> _worker;

        public EffectBase()
        {
            _worker = new TransformBlock<ImageState, ImageState>(DoEffectAsync);
        }

        public ISourceBlock<ImageState> Source => _worker;
        public ITargetBlock<ImageState> Target => _worker;

        protected virtual ImageState DoEffectAsync(ImageState data)
        {
            (ImmutableArray<byte> input, _, _) = data;
            Image<Rgba32> image = Image.Load(input.ToBuilder().ToArray());
            image.Mutate(OnEffect);
            var output = new MemoryStream();
            image.SaveAsJpeg(output);
            return (ImmutableArray.CreateRange(output.ToArray()), data.Topic, data.Index);
        }

        protected abstract void OnEffect(IImageProcessingContext<Rgba32> context);
    }

}
