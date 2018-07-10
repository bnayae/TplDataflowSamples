using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Bnaya.Samples
{
    /// <summary>
    /// Implements dragging operations by catching mouse events
    /// </summary>
    public class DraggingHelper
    {
        #region Fields

        /// <value>Holds state for indicating element if dragged</value>
        private bool _isDragging;

        private readonly FrameworkElement _source;
        /// <value>Holds the parent of the source</value>
        private readonly IInputElement _parent;

        /// <value>Holds the source of the mouse events</value>
        private readonly IInputElement _listItem;

        /// <value>Holds state of the dragging origin point</value>
        private Point _origin;

        /// <value>Holds state of the last dragging point</value>
        private Point _last;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes this instance with an element which is the source of mouse events
        /// </summary>
        public DraggingHelper(FrameworkElement source)
        {
            _source = source;
            Movable = source.DataContext as IMoveable;
            if (Movable == null)
            {
                var factory = source.DataContext as IMoveableFactory;
                if (factory != null)
                    Movable = factory.Moveable;
            }
            this._listItem = FindAncestor<ListBoxItem>(source); ;
            this._parent = VisualTreeHelper.GetParent(Source as DependencyObject) as IInputElement;

            this._source.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            this._listItem.PreviewMouseMove += OnMouseMove;
            this._source.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        #endregion

        #region Movable

        public IMoveable Movable { get; private set; }

        #endregion // Movable

        #region IsDragging

        /// <value>True if dragged, false otherwise</value>
        public bool IsDragging
        {
            get { return _isDragging; }
        }

        #endregion //IsDragging

        #region Source

        /// <value>The source of the mouse events</value>
        public IInputElement Source
        {
            get { return _listItem; }
        }

        #endregion // Source

        #region Origin

        public Point Origin
        {
            get { return _origin; }
        }

        #endregion // Origin

        #region FindAncestor

        public static T FindAncestor<T>(DependencyObject from)
          where T : class
        {
            if (from == null)
            {
                return null;
            }

            T candidate = from as T;
            if (candidate != null)
            {
                return candidate;
            }

            return FindAncestor<T>(VisualTreeHelper.GetParent(from));
        }

        #endregion // FindAncestor

        #region CancelDrag

        /// <summary>
        /// Cancels the drag operation
        /// </summary>
        public void CancelDrag()
        {
            if (!IsDragging)
            {
                return;
            }

            var delta = _last - _origin;
            OnDragCompleted(new MouseDragEventArgs(
                Mouse.MouseMoveEvent,
                Mouse.PrimaryDevice,
                0,
                MouseButton.Left,
                _origin,
                Mouse.GetPosition(_parent),
                delta));

            _origin = new Point();
            _last = new Point();
            _isDragging = false;

            _listItem.ReleaseMouseCapture();
        }

        #endregion // CancelDrag

        #region Events

        /// <summary>
        /// Drag operation begins
        /// </summary>
        public event MouseDragEventHandler DragStarted;

        /// <summary>
        /// Drag operation in progress
        /// </summary>
        public event MouseDragEventHandler DragDelta;

        /// <summary>
        /// Drag operation ended
        /// </summary>
        public event MouseDragEventHandler DragCompleted;

        #region OnDragStarted

        protected virtual void OnDragStarted(MouseDragEventArgs e)
        {
            if (DragStarted != null)
            {
                DragStarted(this, e);
            }
        }

        #endregion // OnDragStarted

        #region OnDragDelta

        protected virtual void OnDragDelta(MouseDragEventArgs e)
        {
            if (DragDelta != null)
            {
                DragDelta(this, e);
            }
        }

        #endregion // OnDragDelta

        #region OnDragCompleted

        protected virtual void OnDragCompleted(MouseDragEventArgs e)
        {
            if (DragCompleted != null)
            {
                DragCompleted(this, e);
            }
        }

        #endregion // OnDragCompleted

        #endregion

        #region Event Handlers

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            if (IsDragging && sender == _source)
            {
                return;
            }

            //_listItem.Focus();
            _source.CaptureMouse();
            _origin = e.GetPosition(_parent);
            _last = _origin;
            _isDragging = true;

            OnDragStarted(new MouseDragEventArgs(
                e.RoutedEvent,
                e.MouseDevice,
                e.Timestamp,
                MouseButton.Left,
                _origin,
                _last,
                new Vector()));

        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = false;
            if (IsDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point current = e.GetPosition(_parent);
                var delta = current - _last;
                OnDragDelta(new MouseDragEventArgs(
                    e.RoutedEvent,
                    e.MouseDevice,
                    e.Timestamp,
                    MouseButton.Left,
                    _origin,
                    current,
                    delta));

                _last = current;
            }
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            if (!IsDragging)
            {
                return;
            }

            Point current = e.GetPosition(_parent);
            var delta = current - _origin;
            OnDragCompleted(new MouseDragEventArgs(
                e.RoutedEvent,
                e.MouseDevice,
                e.Timestamp,
                MouseButton.Left,
                _origin,
                current,
                delta));

            _origin = new Point();
            _last = _origin;
            _isDragging = false;

            _source.ReleaseMouseCapture();
        }

        #endregion

    } // Class DraggingOperations
}
