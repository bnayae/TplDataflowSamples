using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;

namespace Bnaya.Samples
{
    [InheritedExport]
    public interface IScenario : INotifyPropertyChanged
    {
        DataflowVisitor Data { get; }
        string Title { get; }
        FrameworkElement Toolbar { get; }
        double Order { get; }
    }
}
