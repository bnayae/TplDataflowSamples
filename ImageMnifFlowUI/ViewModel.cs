using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Windows.Input;

//  				     		   Only for cats				            
//                                 +----------+		            
//  				     	 +---> | Pixelate |---+	            
//  				     	 |	   +----------+   |	            
//  				     	 |                    |	            
//  +----------------+       |		 Define:	  |     +-----------+	      +---------------+
//  | Dog Downloader |--+--->|	 BoundedCapacity  +---->| Grayscale +----+--->| Write to disk |
//  +----------------+  |    |    				  |     +-----------+	 |    +---------------+
//  				    |	 |					  |	                     |
//                      |    |     +----------+   |	                     |
//  +----------------+	| 	 +---> | OilPaint |---+	                     |     +------------+
//  | Cat Downloader |--+ 	 |	   +----------+	  |            			 +---->| Show on UI |
//  +----------------+	 	 |					  |            			  	   +------------+
//                           |     +----------+	  |             
//  				         +---> | Blur     |---+	            
//  				        	   +----------+		            

// TODO: Immutable array

namespace ImageMnifFlowUI
{
    internal class ViewModel
    {
        private readonly IProgress<byte[]> _sync;
        public ViewModel()
        {
            _sync = new Progress<byte[]>(Images.Add);
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var storage = new Storage();
            var toUI = new ToUI(_sync);
            var grayscale = new Grayscale();
            var fromGrayscale = new BroadcastBlock<(byte[] data, string topic, int index)>(i => i);

            grayscale.Source.LinkTo(fromGrayscale, linkOptions);
            fromGrayscale.LinkTo(storage.Target, linkOptions);
            fromGrayscale.LinkTo(toUI.Target, linkOptions);

            var blur = new GaussianBlur();
            var oilPaint = new OilPaint();
            var pixelate = new Pixelate();
            var toGrayscale = new BroadcastBlock<(byte[] data, string topic, int index)>(i => i);

            toGrayscale.LinkTo(grayscale.Target, linkOptions);
            blur.Source.LinkTo(toGrayscale, linkOptions);
            oilPaint.Source.LinkTo(toGrayscale, linkOptions);
            pixelate.Source.LinkTo(toGrayscale, linkOptions);

            var cats = new Downloader("cat");
            var dogs = new Downloader("dog");
            var toEffects = new BroadcastBlock<(byte[] data, string topic, int index)>(i => i);

            toEffects.LinkTo(blur.Target, linkOptions);
            toEffects.LinkTo(oilPaint.Target, linkOptions);
            toEffects.LinkTo(pixelate.Target, linkOptions, x => x.topic == "dog");
            dogs.Source.LinkTo(toEffects, linkOptions);
            cats.Source.LinkTo(toEffects, linkOptions);
        }

        public ObservableCollection<byte[]> Images { get; } = new ObservableCollection<byte[]>();
    }
}
