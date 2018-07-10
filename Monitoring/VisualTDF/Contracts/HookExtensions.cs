#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples.Hooks
{
    public static class HookExtensions
    {
        private static ThreadLocal<Random> _rnd = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        #region Hook

        public static IPropogateHook<TIn, TOut> Hook<TIn, TOut>(
            this IPropagatorBlock<TIn, TOut> instance, 
            string name, 
            DataflowVisitor visitor, 
            int left = -1,
            int top = -1,
            Color? color = null)
        {
            var hook = new PropogateHook<TIn, TOut>(instance, name, visitor, color);
            Init(hook, left, top);
            return hook;
        }

        #endregion // Hook

        #region HookSource

        public static ISourceHook<T> Hook<T>(
            this ISourceBlock<T> instance, 
            string name,
            DataflowVisitor visitor, 
            int left = -1,
            int top = -1,
            Color? color = null)
        {
            var hook = new SourceHook<T>(instance, name, visitor, color);
            Init(hook, left, top);
            return hook;
        }

        #endregion // HookSource

        #region HookTarget

        public static ITargetHook<T> Hook<T>(
            this ITargetBlock<T> instance, 
            string name, 
            DataflowVisitor visitor,
            int left = -1,
            int top = -1,
            Color? color = null)
        {
            var hook = new TargetHook<T>(instance, name, visitor, color);
            Init(hook, left, top);

            return hook;
        }

        #endregion // HookTarget

        #region Init

        private static void Init(IDataflowHook hook, int left, int top)
        {
            if (left == -1)
                left = _rnd.Value.Next(0, 1400);
            if (top == -1)
                top = _rnd.Value.Next(0, 1000);
            hook.BlockInfo.Location.Left = left;
            hook.BlockInfo.Location.Top = top;
        }

        #endregion // Init
    }
}
