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

namespace Bnaya.Samples
{

    public class Downloader
    {
        private readonly TransformBlock<int, ImageState> _worker;
        private readonly List<string> URLS = new List<string>();

        public Downloader()
        {
            URLS.Add("https://source.unsplash.com/400x400/");
            // URLS.Add("https://petapixel.com/assets/uploads/2019/07/Overall-Winner-and-1st-Oldies-Denise-Czichocki-%C2%A9.jpg");
            // URLS.Add("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcS4E8dq0lh30LtZkMO5iSHqSXK44tgod2tRUp91oZoT-ZxJifHJKA");
            _worker = new TransformBlock<int, ImageState>(DownloadAsync);
            for (int i = 0; i < 20; i++)
            {
                _worker.Post(i);
            }
            _worker.Complete();
        }

        public ISourceBlock<ImageState> Source => _worker;

        private async Task<ImageState> DownloadAsync(int i)
        {
            using (var http = new HttpClient())
            {
                string url = URLS[i % URLS.Count];
                var data = await http.GetByteArrayAsync(url).ConfigureAwait(false);
                var state = new ImageState(data, "", i);
                return state;
            }
        }
    }

}
