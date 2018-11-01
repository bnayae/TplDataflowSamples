using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Simple_Image_Processing_Flow_Exercise
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/1000x1000/?dog/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");
            var option = new ExecutionDataflowBlockOptions { BoundedCapacity = 2 };
            var option1 = new ExecutionDataflowBlockOptions { BoundedCapacity = 2 , MaxDegreeOfParallelism = 3};
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var downloader = new TransformBlock<string, byte[]>(async url =>
            {
                using (var http = new HttpClient())
                {
                    var data = await http.GetByteArrayAsync(url).ConfigureAwait(false);
                    return data;
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 10 });

            var toImage = new TransformBlock<byte[], Image<Rgba32>>(buffer => Image.Load(buffer));

            var grayscale = new TransformBlock<Image<Rgba32>, Image<Rgba32>>(image =>
            {
                image.Mutate(x => x.Grayscale());
                return image;
            });
            var oilPaint = new TransformBlock<Image<Rgba32>, Image<Rgba32>>(image =>
            {
                Console.Write("+OP ");
                image.Mutate(x => x.OilPaint());
                Console.Write("-OP ");
                return image;
            }, option1);
            var pixelate = new TransformBlock<Image<Rgba32>, Image<Rgba32>>(image =>
            {
                Console.Write("+PX ");
                image.Mutate(x => x.Pixelate());
                Console.Write("-PX ");
                return image;
            }, option);
            var blur = new TransformBlock<Image<Rgba32>, Image<Rgba32>>(image =>
            {
                Console.Write("+BL ");
                image.Mutate(x => x.GaussianBlur());
                Console.Write("-BL ");
                return image;
            }, option1);
            var save = new ActionBlock<Image<Rgba32>>(async image =>
            {
                using (var f = new FileStream($"{Guid.NewGuid():N}.jpg",
                                        FileMode.Create, FileAccess.Write, FileShare.None, 4096,
                                        FileOptions.Asynchronous))
                using(var mem = new MemoryStream())
                {
                    image.SaveAsJpeg(mem);
                    mem.Position = 0;
                    await mem.CopyToAsync(f).ConfigureAwait(false);
                }                
            });

            toImage.LinkTo(grayscale, linkOptions);
            grayscale.LinkTo(oilPaint, linkOptions);
            grayscale.LinkTo(blur, linkOptions);
            grayscale.LinkTo(pixelate, linkOptions);
            pixelate.LinkTo(save);
            oilPaint.LinkTo(save);
            blur.LinkTo(save);
            Task _ = Task.WhenAll(pixelate.Completion, oilPaint.Completion, blur.Completion)
                .ContinueWith(c => save.Complete());

            for (int i = 0; i < 30; i++)
            {
                downloader.Post(URL);
            }
            await Task.Delay(4000).ConfigureAwait(false);
            downloader.LinkTo(toImage);
            downloader.Complete();

            await save.Completion.ConfigureAwait(false);
            Console.WriteLine("Done");


            Console.ReadKey();
        }
    }
}
