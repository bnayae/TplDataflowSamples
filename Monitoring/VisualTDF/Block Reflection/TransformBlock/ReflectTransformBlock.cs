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
    // TODO:
    //      TaskForOutputProcessing

    public sealed class ReflectTransformBlock<TIn, TOut> : ReflectSourceBlock<TOut>, IReflectBlock
    {
        private readonly object _targetDebugInfo;
        private readonly object _sourceDebugInfo;

        #region Ctor

        internal ReflectTransformBlock(TransformBlock<TIn, TOut> block, IBlockInitializer propertySetter)
            : base(block, propertySetter)
        {
            if (block == null)
                return;

            #region targetCoreField

            FieldInfo targetCoreField = block.GetType().GetField("m_target", ReflectBlockExtensions.DEFAULT_FLAGS);
            object targetCore = targetCoreField.GetValue(block);
            MethodInfo getTargetDebuggingInformationMethod =
                targetCore.GetType().GetMethod("GetDebuggingInformation", ReflectBlockExtensions.DEFAULT_FLAGS);
            _targetDebugInfo = getTargetDebuggingInformationMethod.Invoke(targetCore, new object[0]);

            #endregion // targetCoreField

            //propertySetter.AddProperty("Input Count", () => block.InputCount, 3, Colors.White);
            //propertySetter.AddProperty("Output Count", () => block.OutputCount, 4, Colors.White);

            propertySetter.SetInputQueueFactory(() => ReflectBlockExtensions.GetInputQueue(_targetDebugInfo));
            //propertySetter.SetOutputQueueFactory(() => ReflectBlockExtensions.GetOutputQueue(_sourceDebugInfo));
            propertySetter.SetPostponedMessagesFactory(() => ReflectBlockExtensions.GetProsponedQueue(_targetDebugInfo));

            propertySetter.AddProperty("Current Parallelism", () => ReflectBlockExtensions.GetCurrentDegreeOfParallelism(_targetDebugInfo), 60, Colors.WhiteSmoke);

            //propertySetter.SetBlockOptions(ReflectBlockExtensions.GetDataflowBlockOptions(_targetDebugInfo));
        }

        #endregion // Ctor
    }
}
