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


namespace Bnaya.Samples
{
    /// <summary>
    /// Reflect SourceCore
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReflectSourceBlock<T> : IReflectBlock
    {

        // TODO: from SourceCore
        //       Task m_taskForOutputProcessing;
        //       Task Completion
        //       ITargetBlock<TOutput> m_nextMessageReservedFor; **
        //       bool m_enableOffering
        //       bool HasExceptions
        //       List<Exception> m_exceptions
        //      SingleProducerSingleConsumerQueue<TOutput> m_messages

        private readonly object _debugInfo;
        private readonly object _sourceCore;

        #region Ctor

        internal ReflectSourceBlock(ISourceBlock<T> block, IBlockInitializer propertySetter)
        {
            if (block == null)
                return;

            var targetType = block.GetType();

            FieldInfo m_sourceField = targetType.GetField("m_source", ReflectBlockExtensions.DEFAULT_FLAGS);
            if (m_sourceField == null)
                return;


            _sourceCore = m_sourceField.GetValue(block);

            if (_sourceCore == null)
                return;

            var getDebuggingInformationMethod = _sourceCore.GetType().GetMethod("GetDebuggingInformation", ReflectBlockExtensions.DEFAULT_FLAGS);
            _debugInfo = getDebuggingInformationMethod.Invoke(_sourceCore, new object[0]);

            InitProperties(block, propertySetter);
        }

        #endregion // Ctor

        #region InitProperties

        protected virtual void InitProperties(ISourceBlock<T> block, IBlockInitializer propertySetter)
        {
            var buffer = block as BufferBlock<T>;
            Func<IEnumerable<string>> outputQueueFactory = () => from item in this.OutputQueue()
                                                                 select item.ToString();
            propertySetter.SetOutputQueueFactory(outputQueueFactory);

            FieldInfo fieldSource = block.GetType().GetField("m_source", ReflectBlockExtensions.DEFAULT_FLAGS);
            object source = fieldSource.GetValue(block);

            PropertyInfo prop =
                _debugInfo.GetType().GetProperty("DataflowBlockOptions", ReflectBlockExtensions.DEFAULT_FLAGS);

            var options = (DataflowBlockOptions)prop.GetValue(_debugInfo);
            propertySetter.SetBlockOptions(options);

            propertySetter.SetIsLinked(IsLinked);

            SetNextMessageIdProperty(source, propertySetter);
            SetTaskForOutputProcessing(propertySetter);

            propertySetter.AddProperty("Is Completed", () => ReflectBlockExtensions.GetIsCompleted(_debugInfo), 61, Colors.WhiteSmoke);
            propertySetter.AddProperty("Declined Permanently", () => ReflectBlockExtensions.GetIsDecliningPermanently(_debugInfo), 62, Colors.WhiteSmoke);
        }

        #endregion // InitProperties

        #region IsLinked

        private bool IsLinked(LinkToInformation linkInfo)
        {
            PropertyInfo fieldLinkedTargets =
                _debugInfo.GetType().GetProperty("LinkedTargets", ReflectBlockExtensions.DEFAULT_FLAGS);
            object targetRegistry = fieldLinkedTargets.GetValue(_debugInfo);
            if (targetRegistry != null)
            {
                PropertyInfo propTargetsForDebugger =
                    targetRegistry.GetType().GetProperty("TargetsForDebugger", ReflectBlockExtensions.DEFAULT_FLAGS);
                object[] targets = propTargetsForDebugger.GetValue(targetRegistry) as IDataflowBlock[];
                return targets.Any(m =>
                    (m as ILinkHook).LinkInfo == linkInfo);
            }
            return false;
        }

        #endregion // IsLinked

        #region SetLastHeaderIdProperty

        private void SetNextMessageIdProperty(object source, IBlockInitializer propertySetter)
        {
            FieldInfo field =
                source.GetType().GetField("m_nextMessageId", ReflectBlockExtensions.DEFAULT_FLAGS);
            propertySetter.AddProperty("Next Header Id", () =>
                {
                    object padded = field.GetValue(source);
                    FieldInfo fieldValue =
                        padded.GetType().GetField("Value", ReflectBlockExtensions.DEFAULT_FLAGS);
                    return fieldValue.GetValue(padded);
                }, 5000, Colors.WhiteSmoke);
        }

        #endregion // SetLastHeaderIdProperty

        #region OutputQueue

        public virtual Func<IEnumerable<T>> OutputQueue
        {
            get
            {
                return () =>
                    {
                        PropertyInfo prop =
                            _debugInfo.GetType().GetProperty("OutputQueue", ReflectBlockExtensions.DEFAULT_FLAGS);

                        var result = prop.GetValue(_debugInfo) as IEnumerable<T>;
                        if (result == null)
                            return Enumerable.Empty<T>();
                        return result;
                    };
            }
        }

        #endregion // OutputQueue

        #region // OutputCount

        //public int OutputCount
        //{
        //    get
        //    {
        //        PropertyInfo prop =
        //            _debugInfo.GetType().GetProperty("OutputCount", ReflectBlockExtensions.DEFAULT_FLAGS);

        //        var result = (int)prop.GetValue(_debugInfo);
        //        return result;
        //    }
        //}

        #endregion // OutputCount

        private void SetTaskForOutputProcessing(IBlockInitializer propertySetter)
        {
            PropertyInfo fieldTaskForInputProcessing =
                _debugInfo.GetType().GetProperty("TaskForOutputProcessing", ReflectBlockExtensions.DEFAULT_FLAGS);
            propertySetter.AddProperty("Is Processing Output",
                () =>
                {
                    Task taskForInputProcessing = fieldTaskForInputProcessing.GetValue(_debugInfo) as Task;
                    if (taskForInputProcessing == null)
                        return false;
                    return taskForInputProcessing.Status == TaskStatus.Running;
                }, 10, Colors.White);
        }
    }
}
