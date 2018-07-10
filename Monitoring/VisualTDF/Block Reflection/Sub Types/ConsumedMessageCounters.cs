using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class ConsumedMessageCounters : INotifyPropertyChanged
    {
        #region Count

        private int _count;

        public int Count
        {
            get { return _count; }
        }

        #endregion // Count

        #region CountReserve

        private int _countReserve;

        public int CountReserve
        {
            get { return _countReserve; }
        }

        #endregion // CountReserve

        #region CountReleaseReservation

        private int _countReleaseReservation;

        public int CountReleaseReservation
        {
            get { return _countReleaseReservation; }
        }

        #endregion // CountReleaseReservation

        #region Increment

        public void Increment(bool succeed)
        {
            Interlocked.Increment(ref _count);
            if (succeed)
            {
                Interlocked.Increment(ref _succeedCount);
                OnPropertyChanged("SucceedCount");
            }
            else
            {
                Interlocked.Increment(ref _failureCount);
                OnPropertyChanged("FailureCount");
            }
            OnPropertyChanged("Count");
        }

        #endregion // Increment

        #region IncrementReserve

        public void IncrementReserve()
        {
            Interlocked.Increment(ref _countReserve);
            OnPropertyChanged("CountReserve");
        }

        #endregion // IncrementReserve

        #region IncrementReleaseReservation

        public void IncrementReleaseReservation()
        {
            Interlocked.Increment(ref _countReleaseReservation);
            OnPropertyChanged("CountReleaseReservation");
        }

        #endregion // IncrementReleaseReservation

        #region SucceedCount

        private int _succeedCount;

        public int SucceedCount
        {
            get { return _succeedCount; }
        }

        #endregion // SucceedCount

        #region FailureCount

        private int _failureCount;

        public int FailureCount
        {
            get { return _failureCount; }
        }

        #endregion // FailureCount

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
