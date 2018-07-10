using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Bnaya.Samples
{
    public class QueuedItem 
    {
        public QueuedItem(string data, Color color)
        {
            Data = data;
            Color = color.ToString();
        }
        public string Data { get; private set; }
        public string Color { get; private set; }
    }
}
