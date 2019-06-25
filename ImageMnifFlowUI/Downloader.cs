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
            URL = $"https://source.unsplash.com/1000x1000/?{topic}/";
            _worker = new TransformBlock<int, (byte[] data, string topic, int index)>(DownloadAsync);
            for (int i = 0; i < 20; i++)
            {
                _worker.Post(i);
            }
            _worker.Complete();
            _topic = topic;
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
