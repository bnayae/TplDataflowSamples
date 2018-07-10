using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Bnaya.Samples
{
    public interface IBlockInitializer
    {
        void AddProperty(string name, Func<object> valueFactory, double order, Color color);
        void AddProperty(string name, object value, double order, Color color);
        void SetInputQueueFactory(Func<IEnumerable<string>> inputQueue);
        void SetOutputQueueFactory(Func<IEnumerable<string>> outputQueue);
        void SetPostponedMessagesFactory(Func<IEnumerable<PostponedMessage>> postponedMessagesFactory);
        void SetIsLinked(Func<LinkToInformation, bool> isLinkedFactory);
    }
}
