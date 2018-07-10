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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Bnaya.Applications.MVVM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            var asm = this.GetType().Assembly;
            var catalog = new AssemblyCatalog(asm);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            SelectedScenario = Scenarios.FirstOrDefault();

            Refresh();
        }

        #region Refresh

        public async void Refresh()
        {
            while (true)
            {
                await Task.Delay(500);
                var selected = SelectedScenario;
                if (selected != null)
                {
                    var data = selected.Data;
                    data.Refresh();
                }
            }
        }

        #endregion // Refresh

        #region Scenarios

        [ImportMany]
        public IScenario[] ScenariosRaw { get; private set; }
        public IEnumerable<IScenario> Scenarios 
        {
            get
            {
                return ScenariosRaw.OrderBy(m => m.Order);
            }
        }

        #endregion // Scenarios

        #region SelectedScenario

        private IScenario _selectedScenario;

        public IScenario SelectedScenario
        {
            get { return _selectedScenario; }
            set
            {
                _selectedScenario = value;
                OnPropertyChanged();
            }
        }

        #endregion // SelectedScenario

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
