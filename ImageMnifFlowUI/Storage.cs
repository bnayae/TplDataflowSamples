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

    public class Storage
    {
        private readonly ActionBlock<(ImmutableArray<byte> data, string topic, int index)> _worker;

        public Storage()
        {
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");
            _worker = new ActionBlock<(ImmutableArray<byte> data, string topic, int index)>(DoSaveAsync);

        }

        public ITargetBlock<(ImmutableArray<byte> data, string topic, int index)> Target => _worker;

        private Task DoSaveAsync((ImmutableArray<byte> buffer, string topic, int index) data)
        {
            (ImmutableArray<byte> buffer, string topic, int index) = data;
            string name = $@"Images\{topic}-{index}.jpg";
            return File.WriteAllBytesAsync(name, buffer.ToBuilder().ToArray());
        }
    }

}
