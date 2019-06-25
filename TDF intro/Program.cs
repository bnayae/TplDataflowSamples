using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TDF_intro
{
    class Program
    {
        private static TransformBlock<int, string> _source;
        private static ActionBlock<string> _target;

        static void Main(string[] args)
        {
            //Intro1();
            Intro2();
            Console.ReadKey();
        }

        private static void Intro2()
        {
            ISourceBlock<string> mock = A.Fake<ISourceBlock<string>>();
            //A.CallTo(() => mock.ConsumeMessage(
            //                A<DataflowMessageHeader>.Ignored,
            //                A<ITargetBlock<string>>.Ignored,
            //                out bool b))
            //                .Returns(false);
                            

            _target = new ActionBlock<string>(async item =>
            {
                Console.WriteLine($"processing {item}");
                await Task.Delay(500).ConfigureAwait(false);
                Console.WriteLine($"processed {item}");
            }, new ExecutionDataflowBlockOptions {  BoundedCapacity = 4 /* internal queue size*/});


            ITargetBlock<string> target = _target;
            for (int i = 1; i <= 10; i++)
            {
                //if(!_target.Post(i.ToString()))
                //    Console.WriteLine($"Drop {i}");
                DataflowMessageStatus response = 
                    target.OfferMessage(
                        new DataflowMessageHeader(i), 
                        i.ToString(),
                        mock,
                        false);
                Console.WriteLine(response);

            }
        }

        private static void Intro1()
        {
            _source = new TransformBlock<int, string>(async i =>
            {
                Console.WriteLine($"transforming {i}");
                await Task.Delay(i * 100).ConfigureAwait(false);
                Console.WriteLine($"transformed {i}");
                return new string('*', i);
            });

            _target = new ActionBlock<string>(async item =>
            {
                Console.WriteLine($"processing {item}");
                await Task.Delay(500).ConfigureAwait(false);
                Console.WriteLine($"processed {item}");
            });

            _source.LinkTo(_target);

            for (int i = 1; i <= 10; i++)
            {
                _source.Post(i);
            }
        }
    }
}
