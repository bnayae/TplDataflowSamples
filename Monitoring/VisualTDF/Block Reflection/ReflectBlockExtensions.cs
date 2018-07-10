#region Using

using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples
{
    public static class ReflectBlockExtensions
    {
        public const BindingFlags DEFAULT_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        #region SetBlockOptions

        public static void SetBlockOptions(this IBlockInitializer blockInitializer, DataflowBlockOptions dataflowBlockOptions)
        {
            blockInitializer.AddProperty("Bounded Capacity", dataflowBlockOptions.BoundedCapacity, 90, Colors.WhiteSmoke);
            blockInitializer.AddProperty("Max Messages Per Task", dataflowBlockOptions.MaxMessagesPerTask, 91, Colors.WhiteSmoke);
            blockInitializer.AddProperty("Scheduler", dataflowBlockOptions.TaskScheduler.GetType().Name, 92, Colors.WhiteSmoke);
            blockInitializer.AddProperty("IsCancelRequested", () => dataflowBlockOptions.CancellationToken.IsCancellationRequested, 110, Colors.WhiteSmoke);
        }

        #endregion // SetBlockOptions

        #region GetInputQueue

        public static IEnumerable<string> GetInputQueue(object debugInfo)
        {
            return GetQueue(debugInfo, "InputQueue");
        }

        #endregion // GetInputQueue

        #region GetOutputQueue

        public static IEnumerable<string> GetOutputQueue(object debugInfo)
        {
            return GetQueue(debugInfo, "OutputQueue");
        }

        #endregion // GetOutputQueue

        #region GetQueue

        private static IEnumerable<string> GetQueue(object debugInfo, string propName)
        {
            PropertyInfo prop =
                debugInfo.GetType().GetProperty(propName, ReflectBlockExtensions.DEFAULT_FLAGS);

            if(prop == null)
                yield break;

            var result = prop.GetValue(debugInfo) as IEnumerable;
            if (result == null)
                yield break;

            foreach (var item in result)
            {
                yield return item.ToString();
            }
        }

        #endregion // GetQueue

        #region GetProsponedQueue

        public static IEnumerable<PostponedMessage> GetProsponedQueue(object debugInfo)
        {
            PropertyInfo prop =
                debugInfo.GetType().GetProperty("PostponedMessages", ReflectBlockExtensions.DEFAULT_FLAGS);

            var kvc = prop.GetValue(debugInfo) as IEnumerable;
            if(kvc == null)
                yield break;

            foreach (var pair in kvc)
	        {
		        dynamic d = pair;
                yield return new PostponedMessage(d.Key, d.Value);
	        }
        }

        #endregion // GetProsponedQueue

        #region GetCurrentDegreeOfParallelism

        public static int GetCurrentDegreeOfParallelism(object debugInfo)
        {
            PropertyInfo prop =
                debugInfo.GetType().GetProperty("CurrentDegreeOfParallelism", ReflectBlockExtensions.DEFAULT_FLAGS);

            var result = (int)prop.GetValue(debugInfo);
            return result;
        }

        #endregion // GetCurrentDegreeOfParallelism

        #region GetDataflowBlockOptions

        public static ExecutionDataflowBlockOptions GetDataflowBlockOptions(object debugInfo)
        {
            PropertyInfo prop =
              debugInfo.GetType().GetProperty("DataflowBlockOptions", ReflectBlockExtensions.DEFAULT_FLAGS);

            var result = (ExecutionDataflowBlockOptions)prop.GetValue(debugInfo);
            return result;
        }

        #endregion // GetDataflowBlockOptions

        #region GetIsDecliningPermanently

        public static bool GetIsDecliningPermanently(object debugInfo)
        {
            PropertyInfo prop =
              debugInfo.GetType().GetProperty("IsDecliningPermanently", ReflectBlockExtensions.DEFAULT_FLAGS);

            var result = (bool)prop.GetValue(debugInfo);
            return result;
        }

        #endregion // GetIsDecliningPermanently

        #region GetIsCompleted

        public static bool GetIsCompleted(object debugInfo)
        {
            PropertyInfo prop =
              debugInfo.GetType().GetProperty("IsCompleted", ReflectBlockExtensions.DEFAULT_FLAGS);

            var result = (bool)prop.GetValue(debugInfo);
            return result;
        }

        #endregion // GetIsCompleted
        
		#region ProcessItem

        public static void ProcessItem(object item, WaitHandle sync, BlockInformation blockInfo)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
                    blockInfo.StartProcessing(item));
            sync.WaitOne();
            Application.Current.Dispatcher.InvokeAsync(() =>
                    blockInfo.EndProcessing(item));
            blockInfo.Refresh();
        }

		#endregion // ProcessItem

        #region IsImplements

        public static bool IsImplements(this object instance, Type @interface)
        {
            return
                     (from interfaceType in instance.GetType().GetInterfaces()
                      where interfaceType.IsGenericType
                      let baseInterface = interfaceType.GetGenericTypeDefinition()
                      where baseInterface == @interface
                      select interfaceType).Any();
        }

        #endregion // IsImplements
    }
}
