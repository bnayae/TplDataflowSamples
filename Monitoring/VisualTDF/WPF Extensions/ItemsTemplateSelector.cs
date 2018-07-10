using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Bnaya.Samples
{
    public class ItemsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is BlockInformation)
                return element.FindResource("BlocksTemplate") as DataTemplate;
            else if (item is LinkToInformation)
                return element.FindResource("LinksToTemplate") as DataTemplate;
            else if (item is ILinkCandidateInformation)
                return element.FindResource("LinksCandidateTemplate") as DataTemplate;
            else if (item is LinkConnector)
                return element.FindResource("ConnectorTemplate") as DataTemplate;

            return null;
        }
    }
}
