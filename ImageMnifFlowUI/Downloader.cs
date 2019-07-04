using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageMnifFlowUI
{

    public class Downloader
    {
        private readonly TransformBlock<int, (byte[] data, string topic, int index)> _worker;
        private readonly string URL;
        private readonly string _topic;

        public Downloader(string topic)
        {
            // URL = $"https://source.unsplash.com/1000x1000/?{topic}/";
            URL = $"https://petapixel.com/assets/uploads/2019/07/Overall-Winner-and-1st-Oldies-Denise-Czichocki-%C2%A9.jpg";
            _worker = new TransformBlock<int, (byte[] data, string topic, int index)>(DownloadAsync);
            _topic = topic;
            for (int i = 0; i < 20; i++)
            {
                _worker.Post(i);
            }
            _worker.Complete();
        }

        public ISourceBlock<(byte[] data, string topic, int index)> Source => _worker;

        private async Task<(byte[] data, string topic, int index)> DownloadAsync(int i)
        {
            using (var http = new HttpClient())
            {
                var data = await http.GetByteArrayAsync(URL).ConfigureAwait(false);
                return (data, _topic, i);
            }
        }
    }

}
