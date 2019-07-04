using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Bnaya.Samples
{
    public class ImageState
    {
        public ImageState(
            byte[] data,
            string topic,
            int index)
        {
            Data = ImmutableArray.CreateRange(data);
            Topic = topic;
            Index = index;
        }
        public ImageState(
            ImmutableArray<byte> data,
            string topic,
            int index)
        {
            Data = data;
            Topic = topic;
            Index = index;
        }

        public ImmutableArray<byte> Data { get; }
        public string Topic { get; }
        public int Index { get; }

        public void Deconstruct(
            out ImmutableArray<byte> data,
            out string topic,
            out int index)
        {
            data = Data;
            topic = Topic;
            index = Index;
        }

        public static implicit operator ImageState(
            (ImmutableArray<byte> data, string topic, int index) image)  // explicit byte to digit conversion operator
        {
            (ImmutableArray<byte> data, string topic, int index) = image;
            return new ImageState(data, topic, index);
        }
    }
}
