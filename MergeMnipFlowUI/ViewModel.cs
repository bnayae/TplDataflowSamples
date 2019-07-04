using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Input;

//  		    						            										
//  		        		            
//                        +----------+		            
//  		        +---> | Pixelate |---+	            
//  		        |	  +----------+   |	            
//  		        |                    |	           
//  +------------+  |	                 |     +-------+	 +------------+
//  | Downloader |--+	                 +---->| Merge |---->| Show on UI |
//  +------------+  |    				 |     +-------+	 +------------+
//                  |					 |	                
//                  |     +----------+   |	                
//                  +---> | Grayscale|---+	                  
//                     	  +----------+	               	  


namespace Bnaya.Samples
{
    internal class ViewModel
    {
        private readonly IProgress<ImmutableArray<byte>> _sync;
        public ViewModel()
        {
            _sync = new Progress<ImmutableArray<byte>>(m => Images.Add(m.ToBuilder().ToArray()));
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var toUI = new ToUI(_sync);
            var merge = new Merge(2);

            merge.Block.LinkTo(toUI.Target, linkOptions);

            var grayscale = new Grayscale();
            var pixelate = new Pixelate();

            grayscale.Source.LinkTo(merge.Block/*, linkOptions*/);
            pixelate.Source.LinkTo(merge.Block/*, linkOptions*/);

            Task _ = ForwardCompletionAsync(merge.Block,
                            grayscale.Source, pixelate.Source);

            var dogs = new Downloader();
            var toEffects = new BroadcastBlock<ImageState>(i => i);

            toEffects.LinkTo(grayscale.Target, linkOptions);
            toEffects.LinkTo(pixelate.Target, linkOptions);
            dogs.Source.LinkTo(toEffects, linkOptions);
        }

        private async Task ForwardCompletionAsync(
            IDataflowBlock blockToComplete,
            params IDataflowBlock[] triggers)
        {
            var tasks = triggers.Select(t => t.Completion);
            await Task.WhenAll(tasks).ConfigureAwait(false);
            blockToComplete.Complete();
        }

        public ObservableCollection<byte[]> Images { get; } = new ObservableCollection<byte[]>();
    }
}
