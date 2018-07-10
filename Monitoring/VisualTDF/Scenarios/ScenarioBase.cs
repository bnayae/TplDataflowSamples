#region Using

using Bnaya.Samples;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Bnaya.Samples.Hooks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reactive;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples.MVVM
{
    public abstract class ScenarioBase : IScenario
    {
        public abstract string Title { get; }
        public abstract double Order { get; }
        public abstract FrameworkElement Toolbar { get; }

        #region Data

        private DataflowVisitor _data;

        public DataflowVisitor Data
        {
            get { return _data; }
            protected set
            {
                _data = value;
                OnPropertyChanged();
            }
        }

        #endregion // Data

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion // INotifyPropertyChanged Members
    }
}
