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

    public class Storage
    {
        private readonly ActionBlock<(byte[] data, string topic, int index)> _worker;

        public Storage()
        {
            _worker = new ActionBlock<(byte[] data, string topic, int index)>(DoSaveAsync);
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");

        }

        public ITargetBlock<(byte[] data, string topic, int index)> Target => _worker;

        private Task DoSaveAsync((byte[] buffer, string topic, int index) data)
        {
            (byte[] buffer, string topic, int index) = data;
            string name = $@"Images\{topic}-{index}.jpg";
            return File.WriteAllBytesAsync(name, buffer);
        }
    }

}
