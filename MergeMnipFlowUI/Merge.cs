using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace Bnaya.Samples
{

    public class Merge
    {
        public Merge(int batchSize)
        {
            var joiner = new BatchBlock<ImageState>(batchSize,
                new GroupingDataflowBlockOptions { Greedy = false });

            var worker = new TransformBlock<ImageState[], ImageState>(DoMerge);
            joiner.LinkTo(worker, 
                new DataflowLinkOptions { PropagateCompletion = true });

            Block = DataflowBlock.Encapsulate(joiner, worker);
        }

        public IPropagatorBlock<ImageState, ImageState> Block { get; }

        private ImageState DoMerge(ImageState[] data)
        {
            Image<Rgba32>[] images = data.Select(m =>
                        {
                            byte[] buffer = m.Data.ToBuilder().ToArray();
                            return Image.Load(buffer);
                        })
                        .ToArray();

            int width = images[0].Width;
            int totalWidth = width * images.Length;
            int height = images[0].Height;
            using (var imageProcessor = new Image<Rgba32>(totalWidth, height))
            using (var outStream = new MemoryStream())
            {
                imageProcessor.Mutate(x => 
                {
                    for (int i = 0; i < images.Length; i++)
                    {
                        var image = images[i];
                    
                        var p = new Point(i * width, 0);
                        x.DrawImage(image, p, 1);
                    }
                });
                imageProcessor.SaveAsJpeg(outStream);
                byte[] manipedImage = outStream.ToArray();
                return new ImageState(manipedImage, data[0].Topic, data[0].Index);
            }
        }
    }

}
