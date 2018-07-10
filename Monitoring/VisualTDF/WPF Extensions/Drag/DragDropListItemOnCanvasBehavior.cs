using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Bnaya.Samples
{
    public class DragDropListItemOnCanvasBehavior : Behavior<FrameworkElement>
    {
        private DraggingHelper _helper;

        protected override void OnAttached()
        {
            _helper = new DraggingHelper(AssociatedObject);
            _helper.DragDelta += OnDragDeltaHandler;

            base.OnAttached();
        }

        private void OnDragDeltaHandler(object sender, MouseDragEventArgs e)
        {
            var movable = _helper.Movable;
            if (movable == null)
                return;
            movable.Location.Left += e.Delta.X;
            movable.Location.Top += e.Delta.Y;
            movable.RefreshLayout();
        }
    }
}
