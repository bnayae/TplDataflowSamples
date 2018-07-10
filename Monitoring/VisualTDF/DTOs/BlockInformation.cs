#region Using

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

#endregion // Using

namespace Bnaya.Samples
{
    [DebuggerDisplay("{Name}")]
    public class BlockInformation : INotifyPropertyChanged, IMoveable, IRefreshable, IBlockInitializer
    {
        private static int _nextId;
        private BindableCollection<string> _processing;
        private Func<IEnumerable<string>> _inputQueue = () => Enumerable.Empty<string>();
        private Func<IEnumerable<string>> _outputQueue = () => Enumerable.Empty<string>();

        #region Ctor

        public BlockInformation(string name, 
            bool isSourceBlock,
            bool isTargetBlock, 
            string color)
        {
            Commands = new BindableCollection<GenericCommand>();
            Id = Interlocked.Increment(ref _nextId);
            Name = name;
            Location = new Location();
            LinksTo = new BindableCollection<LinkToInformation>();
            _processing = new BindableCollection<string>();
            IsSourceBlock = isSourceBlock;
            IsTargetBlock = isTargetBlock;
            Color = color;
            PushOfferingCounter = new OfferMessageCounters();
            PushOffering = new BindableCollection<OfferMessageTrace>();
            PoolRequestCounters = new ConsumedMessageCounters();
            PoolRequest = new BindableCollection<ConsumeTrace>();
        }

        #endregion // Ctor

        public BindableCollection<LinkToInformation> LinksTo { get; private set; }
        public readonly SortedList<double, PropertyData> _properties = new SortedList<double,PropertyData>();
        public PropertyData[] _propertiesCache;
        public BindableCollection<GenericCommand> Commands { get; private set; }

        #region PushOffering

        /// <summary>
        /// Gets the offer request, made by other (linked) sources.
        /// different sources can offered messages to this target. 
        /// </summary>
        /// <value>
        /// The push offering.
        /// </value>
        public BindableCollection<OfferMessageTrace> PushOffering { get; private set; }

        #endregion // PushOffering

        #region PoolRequest

        /// <summary>
        /// Gets the consumed requests, made by other targets.
        /// different targets can consume messages from thin source.
        /// </summary>
        /// <value>
        /// The pool request.
        /// </value>
        public BindableCollection<ConsumeTrace> PoolRequest { get; private set; }   // messages can consumed from different source

        #endregion // PoolRequest

        #region PushOfferingCounter

        /// <summary>
        /// Gets the offer request counters, made by other (linked) sources.
        /// different sources can offered messages to this target. 
        /// </summary>
        /// <value>
        /// The offer counters. 
        /// </value>
        public OfferMessageCounters PushOfferingCounter { get; private set; }

        #endregion // PushOfferingCounter

        #region PoolRequestCounters

        /// <summary>
        /// Gets the consumed requests counters, made by other targets.
        /// different targets can consume messages from thin source.
        /// </summary>
        /// <value>
        /// The consumed counters.
        /// </value>
        public ConsumedMessageCounters PoolRequestCounters { get; private set; }

        #endregion // PoolRequestCounters

        #region Name

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        #endregion // Name

        #region Id

        public int Id { get; private set; }

        #endregion // Id

        #region Color

        public string Color { get; private set; }

        #endregion // Color

        #region SetInputQueueFactory

        public void SetInputQueueFactory(Func<IEnumerable<string>> inputQueue)
        {
            _inputQueue = inputQueue;
        }

        #endregion // SetInputQueueFactory

        #region SetOutputQueueFactory

        public void SetOutputQueueFactory(Func<IEnumerable<string>> outputQueue)
        {
            _outputQueue = outputQueue;
        }

        #endregion // SetOutputQueueFactory
        
        #region Queues

        public IEnumerable<QueuedItem> Queues
        {
            get 
            {
                if (_inputQueue == null)
                    return Enumerable.Empty<QueuedItem>();

                var input = from item in _inputQueue()
                            select new QueuedItem(item, Colors.DarkGreen);

                var processing = from item in _processing
                            select new QueuedItem(item, Colors.IndianRed);

                var output = from item in _outputQueue()
                            select new QueuedItem(item, Colors.DarkBlue);

                return input.Concat(processing).Concat(output);
            }
        }

        #endregion // Queues

        #region PostponedMessages

        private Func<IEnumerable<PostponedMessage>> _postponedMessagesFactory;
        public void SetPostponedMessagesFactory(Func<IEnumerable<PostponedMessage>> postponedMessagesFactory)
        { 
            _postponedMessagesFactory = postponedMessagesFactory;
        }
        public IEnumerable<PostponedMessage> PostponedMessages
        {
            get
            {
                if (_postponedMessagesFactory == null)
                    return Enumerable.Empty<PostponedMessage>();
                return _postponedMessagesFactory();
            }
        }

        #endregion // PostponedMessages

        #region IsLinked

        private Func<LinkToInformation, bool> _isLinkedFactory;
        public void SetIsLinked(Func<LinkToInformation, bool> isLinkedFactory)
        {
            _isLinkedFactory = isLinkedFactory;
        }
        public bool IsLinked(LinkToInformation link)
        {
            if (_isLinkedFactory == null)
                return false;
            return _isLinkedFactory(link);
        }

        #endregion // IsLinked

        #region Location

        /// <summary>
        /// Gets the location (for UI layout).
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Location Location { get; private set; }

        #endregion // Location

        #region IsSourceBlock

        /// <summary>
        /// Gets a value indicating whether [is source block].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is source block]; otherwise, <c>false</c>.
        /// </value>
        public bool IsSourceBlock { get; private set; }

        #endregion // IsSourceBlock

        #region IsTargetBlock

        public bool IsTargetBlock { get; private set; }

        #endregion // IsTargetBlock

        #region State

        private BlockState _state = BlockState.Active;

        public BlockState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        #endregion // State

        #region StartProcessing

        public void StartProcessing(object data)
        {
            string item = (data ?? "NULL").ToString();
            _processing.Add(item);
            OnPropertyChanged("Queues");
        }

        #endregion // StartProcessing

        #region EndProcessing

        public void EndProcessing(object data)
        {
            string item = (data ?? "NULL").ToString();
            _processing.Remove(item);
            OnPropertyChanged("Queues");
        }

        #endregion // EndProcessing

        #region Refresh

        public void Refresh()
        {
            foreach (var p in _properties)
            {
                p.Value.Refresh();
            }
            OnPropertyChanged("Queues");
            OnPropertyChanged("PostponedMessages");
        }

        #endregion // Refresh

        #region DynamicProperties

        public IEnumerable<PropertyData> Properties
        {
            get 
            {
                if (_propertiesCache == null)
                    _propertiesCache = _properties.Select(m => m.Value).ToArray();
                return _propertiesCache;
            }
        }

        #endregion // Properties

        #region AddProperty

        public void AddProperty(string name, object value, double order, Color color)
        {
            var p = new PropertyData(name, value, color.ToString());
            _properties.Add(order, p);
            _propertiesCache = null;
        }

        public void AddProperty(string name, Func<object> valueFactory, double order, Color color)
        {
            var p = new PropertyData(name, valueFactory, color.ToString());
            _properties.Add(order, p);
            _propertiesCache = null;
        }

        #endregion // AddProperty

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

        #region CommandsVisibility

        private bool _commandsVisibility = true;

        public bool CommandsVisibility
        {
            get { return _commandsVisibility; }
            set
            {
                _commandsVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // CommandsVisibility

        #region StateVisibility

        private bool _stateVisibility = true;

        public bool StateVisibility
        {
            get { return _stateVisibility; }
            set
            {
                _stateVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // StateVisibility

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

        #region RefreshLayout

        void IMoveable.RefreshLayout()
        {
        }

        #endregion // RefreshLayout

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
