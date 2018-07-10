using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    public class OfferMessageCounters : INotifyPropertyChanged
    {
        #region Count

        private int _count;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }

        #endregion // Count

        #region Increment

        public void Increment(DataflowMessageStatus result)
        {
            Interlocked.Increment(ref _count);
            OnPropertyChanged("Count");
            switch (result)
            {
                case DataflowMessageStatus.Accepted:
                    Interlocked.Increment(ref _acceptedCount);
                    OnPropertyChanged("AcceptedCount");
                    break;
                case DataflowMessageStatus.Postponed:
                    Interlocked.Increment(ref _postponedCount);
                    OnPropertyChanged("PostponedCount");
                    break;
                case DataflowMessageStatus.Declined:
                    Interlocked.Increment(ref _declinedCount);
                    OnPropertyChanged("DeclinedCount");
                    break;
                case DataflowMessageStatus.DecliningPermanently:
                    Interlocked.Increment(ref _decliningPermanentlyCount);
                    OnPropertyChanged("DecliningPermanentlyCount");
                    break;
                case DataflowMessageStatus.NotAvailable:
                    Interlocked.Increment(ref _notAvailableCount);
                    OnPropertyChanged("NotAvailableCount");
                    break;
                default:
                    break;
            }
        }

        #endregion // Increment

        #region AcceptedCount

        private int _acceptedCount;

        public int AcceptedCount
        {
            get { return _acceptedCount; }
        }

        #endregion // AcceptedCount

        #region DeclinedCount

        private int _declinedCount;

        public int DeclinedCount
        {
            get { return _declinedCount; }
        }

        #endregion // DeclinedCount

        #region PostponedCount

        private int _postponedCount;

        public int PostponedCount
        {
            get { return _postponedCount; }
        }

        #endregion // PostponedCount

        #region NotAvailableCount

        private int _notAvailableCount;

        public int NotAvailableCount
        {
            get { return _notAvailableCount; }
        }

        #endregion // NotAvailableCount

        #region DecliningPermanentlyCount

        private int _decliningPermanentlyCount;

        public int DecliningPermanentlyCount
        {
            get { return _decliningPermanentlyCount; }
        }

        #endregion // DecliningPermanentlyCount

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
