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

    public abstract class EffectBase
    {
        private readonly TransformBlock<(byte[] data, string topic, int index), (byte[] data, string topic, int index)> _worker;

        public EffectBase()
        {
            _worker = new TransformBlock<(byte[] data, string topic, int index), (byte[] data, string topic, int index)>(DoEffectAsync);
        }

        public ISourceBlock<(byte[] data, string topic, int index)> Source => _worker;
        public ITargetBlock<(byte[] data, string topic, int index)> Target => _worker;

        private (byte[] data, string topic, int index) DoEffectAsync((byte[] input, string topic, int index) data)
        {
            (byte[] input, _, _) = data;
            Image<Rgba32> image = Image.Load(input);
            image.Mutate(OnEffect);
            var output = new MemoryStream();
            image.SaveAsJpeg(output);
            return (output.ToArray(), data.topic, data.index);
        }

        protected abstract void OnEffect(IImageProcessingContext<Rgba32> context);
    }

}
