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
    public sealed class ReflectBufferBlock<T> : ReflectSourceBlock<T>, IReflectBlock
    {

        // TODO: from SourceCore
        //       Task m_taskForOutputProcessing;
        //       Task Completion
        //       ITargetBlock<TOutput> m_nextMessageReservedFor; **
        //       bool m_enableOffering
        //       bool HasExceptions
        //       List<Exception> m_exceptions

        private readonly object _debugInfo;

        #region Ctor

        internal ReflectBufferBlock(BufferBlock<T> block, IBlockInitializer propertySetter)
            : base (block, propertySetter)
        {
            //propertySetter.AddProperty("OutputCount", () => block.Count, 6, Colors.White);

            SetBoundingState(block, propertySetter);
        }

        #endregion // Ctor

        #region SetBoundingState

        private void SetBoundingState(IDataflowBlock block, IBlockInitializer propertySetter)
        {
            FieldInfo field =
                block.GetType().GetField("m_boundingState", ReflectBlockExtensions.DEFAULT_FLAGS);
            object boundingState = field.GetValue(block);
            if (boundingState != null)
            {
                #region TaskForInputProcessing

                FieldInfo fieldTaskForInputProcessing =
                    boundingState.GetType().GetField("TaskForInputProcessing", ReflectBlockExtensions.DEFAULT_FLAGS);
                propertySetter.AddProperty("Is Processing Input",
                    () =>
                    {
                        Task taskForInputProcessing = fieldTaskForInputProcessing.GetValue(boundingState) as Task;
                        if (taskForInputProcessing == null)
                            return false;
                        return taskForInputProcessing.Status == TaskStatus.Running;
                    }, 9, Colors.White);

                #endregion // TaskForInputProcessing

                #region PostponedMessages

                FieldInfo fieldPostponedMessages =
                    boundingState.GetType().GetField("PostponedMessages", ReflectBlockExtensions.DEFAULT_FLAGS);

                Func<IEnumerable<PostponedMessage>> postponedMessagesFactory = () =>
                {
                    var kvc = fieldPostponedMessages.GetValue(boundingState) as IEnumerable<KeyValuePair<ISourceBlock<T>, DataflowMessageHeader>>;

                    var result = from pair in kvc
                                 select new PostponedMessage(pair.Key, pair.Value);

                    return result;
                };

                propertySetter.SetPostponedMessagesFactory(postponedMessagesFactory);

                #endregion // PostponedMessages
            }
        }

        #endregion // SetBoundingState
    }
}
