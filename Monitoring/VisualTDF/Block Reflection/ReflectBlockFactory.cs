#region Using

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples
{
    public sealed class ReflectBlockFactory
    {
        public static IReflectBlock Create<TIn, TOut>(IPropagatorBlock<TIn, TOut> block, IBlockInitializer propertySetter)
        {
            if (block is TransformBlock<TIn, TOut>)
            {
                var reflectView = new ReflectTransformBlock<TIn, TOut>(block as TransformBlock<TIn, TOut>, propertySetter);
                return reflectView;
            }
            else if (block is BufferBlock<TIn>)
            {
                var reflectView = new ReflectBufferBlock<TIn>(block as BufferBlock<TIn>, propertySetter);
                return reflectView;
            }
            else
                return null;           
        }
        public static IReflectBlock Create<T>(IDataflowBlock block, IBlockInitializer propertySetter)
        {
            if (block is ActionBlock<T>)
            {
                var reflectView = new ReflectActionBlock<T>(block, propertySetter);
                return reflectView;
            }
            else
                return null;           
        }
    }
}
