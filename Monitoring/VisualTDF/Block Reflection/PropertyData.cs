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
    // TODO: apply property hierarchic / grouping
    [DebuggerDisplay("{Name}: {Value}")]
    public class PropertyData : INotifyPropertyChanged
    {
        private readonly Func<object> _valueFactory;

        #region Ctor

        public PropertyData(string name, object value, string color)
        {
            Name = name;
            Value = value;
            Color = color;
        }

        public PropertyData(string name, Func<object> valueFactory, string color)
        {
            Name = name;
            _valueFactory = valueFactory;
            Value = _valueFactory();
            Color = color;
        }

        #endregion // Ctor

        public string Name { get; private set; }

        #region Value

        private object _value;

        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        #endregion // Value

        public string Color { get; set; }

        #region Refresh

        public void Refresh()
        {
            if (_valueFactory == null)
                return;
            Value = _valueFactory();
        }

        #endregion // Refresh

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
