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

namespace Bnaya.Samples
{

    public class Pixelate: EffectBase
    {
        protected override void OnEffect(
            IImageProcessingContext<Rgba32> context)
        {
            context.Pixelate(15);
        }
    }
}
