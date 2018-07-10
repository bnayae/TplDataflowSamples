using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Bnaya.Samples
{
    public delegate void MouseDragEventHandler(object sender, MouseDragEventArgs e);

    public class MouseDragEventArgs : MouseEventArgs
    {
        private readonly MouseButton _button;
        private readonly Point _origin;
        private readonly Point _current;
        private readonly Vector _delta;

        internal MouseDragEventArgs(
            RoutedEvent routedEvent,
            MouseDevice mouse, int timestamp,
            MouseButton button, Point origin, Point current, Vector delta)
            : base(mouse, timestamp)
        {
            this.RoutedEvent = routedEvent;
            this._button = button;
            this._origin = origin;
            this._current = current;
            this._delta = delta;
        }

        public MouseButton Button
        {
            get { return _button; }
        }

        public Point Origin
        {
            get { return _origin; }
        }

        public Point Current
        {
            get { return _current; }
        }

        public Vector Delta
        {
            get { return _delta; }
        }
    }
}
