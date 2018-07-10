#region Using

using Bnaya.Samples.Hooks;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;

#endregion // Using

namespace Bnaya.Samples
{
    public class LinkCandidateInformation<T> : INotifyPropertyChanged, IMoveableFactory, ILinkCandidateInformation
    {
        private DataflowVisitor _visitor;
        private readonly ISourceHook<T> _source;
        private readonly ITargetHook<T> _target;
        private readonly Predicate<T> _predicate;

        #region Ctor

        #region Overloads

        public LinkCandidateInformation(
            DataflowVisitor visitor,
            ISourceHook<T> source,
            ITargetHook<T> target,
            DataflowLinkOptions options = null,
            Predicate<T> predicate = null)
            : this(visitor, source, target, 
                    new LinkConnector(source.BlockInfo, target.BlockInfo), 
                    options, predicate)
        {
        }

        #endregion // Overloads

        public LinkCandidateInformation(
            DataflowVisitor visitor,
            ISourceHook<T> source,
            ITargetHook<T> target,
            LinkConnector connector,
            DataflowLinkOptions options = null,
            Predicate<T> predicate = null)
        {
            _visitor = visitor;
            _source = source;
            _target = target;
            _predicate = predicate;

            if (options == null)
                options = new DataflowLinkOptions();
            Append = options.Append;
            MaxMessages = options.MaxMessages;
            PropagateCompletion = options.PropagateCompletion;

            Connector = connector;
        }

        #endregion // Ctor

        #region AttachCommand

        public void AttachCommand()
        {
            OptionsVisibility = false;
            var option = new DataflowLinkOptions{ Append = _append, MaxMessages = _maxMessages, PropagateCompletion = _propagateCompletion};
            _visitor.RemoveLinkCandidate(this);
            var hook = new LinkHook<T>(_source, _target, option, _visitor, _predicate, this);
            _visitor.AddLink(hook.LinkInfo);
        }

        #endregion // AttachCommand

        #region Connector

        public LinkConnector Connector { get; internal set; }

        #endregion // Connector

        #region Moveable

        IMoveable IMoveableFactory.Moveable
        {
            get { return Connector; }
        }

        #endregion // Moveable

        #region Append

        private bool _append = true;

        public bool Append
        {
            get { return _append; }
            set
            {
                _append = value;
                OnPropertyChanged();
            }
        }

        #endregion // Append

        #region MaxMessages

        private int _maxMessages = -1;

        public int MaxMessages
        {
            get { return _maxMessages; }
            set
            {
                _maxMessages = value;
                OnPropertyChanged();
            }
        }

        #endregion // MaxMessages

        #region PropagateCompletion

        private bool _propagateCompletion;

        public bool PropagateCompletion
        {
            get { return _propagateCompletion; }
            set
            {
                _propagateCompletion = value;
                OnPropertyChanged();
            }
        }

        #endregion // PropagateCompletion

        #region OptionsVisibility

        private bool _optionsVisibility;

        public bool OptionsVisibility
        {
            get { return _optionsVisibility; }
            set
            {
                _optionsVisibility = value;
                OnPropertyChanged();
            }
        }

        #endregion // OptionsVisibility

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
