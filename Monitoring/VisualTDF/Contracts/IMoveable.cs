using Bnaya.Samples;
using System;
namespace Bnaya.Samples
{
    public interface IMoveable
    {
        Location Location { get; }

        void RefreshLayout();
    }
}
