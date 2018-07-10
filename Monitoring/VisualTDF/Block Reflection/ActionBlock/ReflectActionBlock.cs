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
    public sealed class ReflectActionBlock<T> : IReflectBlock
    {
        private readonly object _debugInfo;

        #region Ctor

        internal ReflectActionBlock(IDataflowBlock block, IBlockInitializer propertySetter)
        {
            if (block == null)
                return;

            var targetType = block.GetType();

            object debugViewProvider = null;
            FieldInfo debugViewProviderField = targetType.GetField("m_defaultTarget", ReflectBlockExtensions.DEFAULT_FLAGS);
            if (debugViewProviderField != null)
            {
                debugViewProvider = debugViewProviderField.GetValue(block);
            }
            if (debugViewProvider == null)
            {
                debugViewProviderField = targetType.GetField("m_spscDebugInfo", ReflectBlockExtensions.DEFAULT_FLAGS);
                if (debugViewProviderField != null)
                    debugViewProvider = debugViewProviderField.GetValue(block);
            }
            if (debugViewProvider == null)
                return;

            MethodInfo getDebuggingInformationMethod =
                debugViewProvider.GetType().GetMethod("GetDebuggingInformation", ReflectBlockExtensions.DEFAULT_FLAGS);

            _debugInfo = getDebuggingInformationMethod.Invoke(debugViewProvider, new object[0]);
            
            InitProperties(block, propertySetter);
        }

        #endregion // Ctor

        #region InitProperties

        private void InitProperties(IDataflowBlock block, IBlockInitializer propertySetter)
        {
            propertySetter.SetInputQueueFactory(() => ReflectBlockExtensions.GetInputQueue(_debugInfo));
            propertySetter.SetPostponedMessagesFactory(() => ReflectBlockExtensions.GetProsponedQueue(_debugInfo));
            
            propertySetter.AddProperty("Current Parallelism", () => ReflectBlockExtensions.GetCurrentDegreeOfParallelism(_debugInfo), 60, Colors.WhiteSmoke);
            propertySetter.AddProperty("Is Completed", () => ReflectBlockExtensions.GetIsCompleted(_debugInfo), 61, Colors.WhiteSmoke);
            propertySetter.AddProperty("Declined Permanently", () => ReflectBlockExtensions.GetIsDecliningPermanently(_debugInfo), 62, Colors.WhiteSmoke);


            propertySetter.SetBlockOptions(ReflectBlockExtensions.GetDataflowBlockOptions(_debugInfo));
        }

        #endregion // InitProperties
    }
}
