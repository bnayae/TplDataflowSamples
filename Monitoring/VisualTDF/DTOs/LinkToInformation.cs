using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace Bnaya.Samples
{
    public class LinkToInformation : INotifyPropertyChanged, IRefreshable, IMoveableFactory
    {
        private System.Action _unlink;

        #region Ctor

        public LinkToInformation(
            BlockInformation source, 
            BlockInformation target,
            DataflowLinkOptions linkOptions,
            System.Action unlink)
            : this(new LinkConnector(source, target), linkOptions, unlink)
        {
        }

        public LinkToInformation(
            LinkConnector connector,
            DataflowLinkOptions linkOptions,
            System.Action unlink)
        {
            Id = Guid.NewGuid();
            Connector = connector;
            LinkOptions = linkOptions;
            _unlink = unlink;

            PushOffering = new BindableCollection<OfferMessageTrace>();
            PoolRequest = new BindableCollection<ConsumeTrace>();
            PushOfferingCounter = new OfferMessageCounters();
            PoolRequestCounters = new ConsumedMessageCounters();
        }

        #endregion // Ctor

        #region Id

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; private set; }

        #endregion // Id

        #region LinkOptions

        public DataflowLinkOptions LinkOptions { get; private set; }

        #endregion // LinkOptions

        #region PushOffering

        /// <summary>
        /// Gets the offer request (upon specific link).
        /// </summary>
        /// <value>
        /// The push offering.
        /// </value>
        public BindableCollection<OfferMessageTrace> PushOffering { get; private set; }

        #endregion // PushOffering

        #region PoolRequest

        /// <summary>
        /// Gets the pool (consume) request (upon specific link).
        /// </summary>
        /// <value>
        /// The pool request.
        /// </value>
        public BindableCollection<ConsumeTrace> PoolRequest { get; private set; }

        #endregion // PoolRequest

        #region PushOfferingCounter

        /// <summary>
        /// Gets the push offering counter (offer message).
        /// </summary>
        /// <value>
        /// The push offering counter.
        /// </value>
        public OfferMessageCounters PushOfferingCounter { get; private set; }

        #endregion // PushOfferingCounter

        #region PoolRequestCounters

        /// <summary>
        /// Gets the pool (consume) request counters.
        /// </summary>
        /// <value>
        /// The pool request counters.
        /// </value>
        public ConsumedMessageCounters PoolRequestCounters { get; private set; }

        #endregion // PoolRequestCounters

        #region InfoVisibility

        private bool _infoVisibility;

        public bool InfoVisibility
        {
            get { return _infoVisibility; }
            set
            {
                _infoVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // InfoVisibility

        #region OfferingVisibility

        private bool _offeringVisibility = true;

        public bool OfferingVisibility
        {
            get { return _offeringVisibility; }
            set
            {
                _offeringVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // OfferingVisibility

        #region PoolRequestVisibility

        private bool _poolRequestVisibility = true;

        public bool PoolRequestVisibility
        {
            get { return _poolRequestVisibility; }
            set
            {
                _poolRequestVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // PoolRequestVisibility

        #region GeneralInfoVisibility

        private bool _generalInfoVisibility;

        public bool GeneralInfoVisibility
        {
            get { return _generalInfoVisibility; }
            set
            {
                _generalInfoVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // GeneralInfoVisibility

        #region Refresh

        public void Refresh()
        {
            bool isLinked = Connector.Source.IsLinked(this);
            if (!isLinked)
                UnlinkCommand();
        }

        #endregion // Refresh

        #region Connector

        public LinkConnector Connector { get; internal set; }

        #endregion // Connector

        #region Moveable

        IMoveable IMoveableFactory.Moveable
        {
            get { return Connector; }
        }

        #endregion // Moveable

        #region UnlinkCommand

        public void UnlinkCommand()
        {
            _unlink();
        }

        #endregion // UnlinkCommand

        #region INotifyPropertyChanged Members

        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }


        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = _propertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion // INotifyPropertyChanged Members

        #region ToString

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Connector.Source.Name, Connector.Target.Name);
        }

        #endregion // ToString
    }
}
