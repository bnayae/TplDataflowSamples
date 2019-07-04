using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly IProgress<ImmutableArray<byte>> _sync;
        public ViewModel()
        {
            _sync = new Progress<ImmutableArray<byte>>(Images.Add);
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var storage = new Storage();
            var toUI = new ToUI(_sync);
            var grayscale = new Grayscale();
            var fromGrayscale = new BroadcastBlock<(ImmutableArray<byte> data, string topic, int index)>(i => i);

            grayscale.Source.LinkTo(fromGrayscale, linkOptions);
            fromGrayscale.LinkTo(storage.Target, linkOptions);
            fromGrayscale.LinkTo(toUI.Target, linkOptions);

            var blur = new GaussianBlur();
            var oilPaint = new OilPaint();
            var pixelate = new Pixelate();

            blur.Source.LinkTo(grayscale.Target/*, linkOptions*/);
            oilPaint.Source.LinkTo(grayscale.Target/*, linkOptions*/);
            pixelate.Source.LinkTo(grayscale.Target/*, linkOptions*/);

            Task.WhenAll(
                        blur.Source.Completion,
                        oilPaint.Source.Completion,
                        pixelate.Source.Completion)
                .ContinueWith(c => grayscale.Target.Complete());
            //Task _ = ForwardCompletionAsync(grayscale.Target,
            //                blur.Source, oilPaint.Source /*oilPaint.Target*/, pixelate.Source);

            var cats = new Downloader("cat");
            var dogs = new Downloader("dog");
            var toEffects = new BroadcastBlock<(ImmutableArray<byte> data, string topic, int index)>(i => i);

            toEffects.LinkTo(blur.Target, linkOptions);
            toEffects.LinkTo(oilPaint.Target, linkOptions);
            toEffects.LinkTo(pixelate.Target, linkOptions, x => x.topic == "cat");
            dogs.Source.LinkTo(toEffects/*, linkOptions*/);
            cats.Source.LinkTo(toEffects/*, linkOptions*/);
            Task _ = ForwardCompletionAsync(toEffects,
                            dogs.Source, cats.Source );
        }

        private async Task ForwardCompletionAsync(
            IDataflowBlock blockToComplete,
            params IDataflowBlock[] triggers)
        {
            var tasks = triggers.Select(t => t.Completion);
            await Task.WhenAll(tasks).ConfigureAwait(false);
            blockToComplete.Complete();
        }

        public ObservableCollection<ImmutableArray<byte>> Images { get; } = new ObservableCollection<ImmutableArray<byte>>();
    }
}
