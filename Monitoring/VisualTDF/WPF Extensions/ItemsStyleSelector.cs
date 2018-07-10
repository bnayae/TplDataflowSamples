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
    public class ItemsStyleSelector : StyleSelector
    {
        public Style BlockStyle { get; set; }
        public Style LinkStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            bool isBlock = item is BlockInformation;

            return isBlock
                       ? BlockStyle
                       : LinkStyle;
        }
    }
}
