using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
/*  Nugets (beta)

SixLabors.ImageSharp
SixLabors.Fonts
SixLabors.Shapes.Text
SixLabors.ImageSharp.Drawing

*/
namespace ImageManipFlow
{
    class Program
    {

        private const string DOGS = "https://source.unsplash.com/1000x1000/?dog/";
        private const string CATS = "https://source.unsplash.com/1000x1000/?cat/";

        static async Task Main(string[] args)
        {
            using (var http = new HttpClient())
            {
                var buffer = await http.GetByteArrayAsync(DOGS).ConfigureAwait(false);
                using (Image<Rgba32> imageProcessor = Image.Load(buffer))
                using (var srm = new FileStream("img.jpg", FileMode.Create,
                    FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
                {
                    imageProcessor.Mutate(x => x
                                        .OilPaint()
                                        .Grayscale()
                                        .GaussianBlur()
                                        .Pixelate(20));
                    imageProcessor.SaveAsJpeg(srm);
                }
            }

            Console.WriteLine("Done");
        }
    }
}
