using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class Location : INotifyPropertyChanged
    {
        #region Left

        private double _left;

        public double Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged();
                OnPropertyChanged("X");
            }
        }

        #endregion // Left

        #region Top

        private double _top;

        public double Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged();
                OnPropertyChanged("Y");
            }
        }

        #endregion // Top

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
