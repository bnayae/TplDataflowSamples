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

namespace Bnaya.Samples
{
    public class LinkConnector : INotifyPropertyChanged, IMoveable, IRefreshable
    {
        #region Ctor

        public LinkConnector(
            BlockInformation source, 
            BlockInformation target)
        {
            Source = source;
            Target = target;
            
            Location = new Location { Left = 0, Top = 0 };
        }

        #endregion // Ctor

        #region Source

        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public BlockInformation Source { get; private set; }

        #endregion // Source

        #region Target

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public BlockInformation Target { get; private set; }

        #endregion // Target

        #region Location

        /// <summary>
        /// Gets the location (for UI layout).
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public Location Location { get; private set; }

        #endregion // Location

        #region ControlLocation

        public Location ControlLocation
        {
            get { return new Samples.Location { Left = MiddleX + Location.Left, Top = MiddleY + Location.Top }; }
        }

        #endregion // ControlLocation

        #region MiddleX

        public double MiddleX
        {
            get 
            {
                double t = Target.Location.Left;
                double s = Source.Location.Left;
                var min = Math.Min(s, t);
                var max = Math.Max(s, t);
                return min + (max - min) / 2;
            }
        }

        #endregion // MiddleX

        #region MiddleY

        public double MiddleY
        {
            get
            {
                double t = Target.Location.Top;
                double s = Source.Location.Top;
                var min = Math.Min(s, t);
                var max = Math.Max(s, t);
                return min + (max - min) / 2;
            }
        }

        #endregion // MiddleY

        #region ConnercorPoints

        public Point[] ConnercorPoints
        {
            get 
            {
                return new Point[]
                    {
                        new Point(Source.Location.Left, Source.Location.Top),
                        new Point(MiddleX + Location.Left * 2, MiddleY + Location.Top * 2),
                        new Point(Target.Location.Left, Target.Location.Top)
                    };
            }
        }

        #endregion // ConnercorPoints

        #region RefreshLayout

        public void RefreshLayout()
        {
            var org = ConnercorPoints;
            OnPropertyChanged("MiddleX");
            OnPropertyChanged("MiddleY");
            OnPropertyChanged("ConnercorPoints");
            OnPropertyChanged("ControlLocation");
        }

        #endregion // RefreshLayout

        #region Thickness

        public int Thickness
        {
            get
            {
                if (Target.PostponedMessages.Any(m => m.Link.Connector == this))
                    return 2;

                return 1;
            }
        }

        #endregion // Thickness

        #region Refresh

        public void Refresh()
        {
            OnPropertyChanged("Thickness");
        }

        #endregion // Refresh

        #region INotifyPropertyChanged Members

        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
                Source.Location.PropertyChanged += PropertyChangedLocationPropagator;
                Target.Location.PropertyChanged += PropertyChangedLocationPropagator;
            }
            remove
            {
                _propertyChanged -= value;
                Source.Location.PropertyChanged -= PropertyChangedLocationPropagator;
                Target.Location.PropertyChanged -= PropertyChangedLocationPropagator;
            }
        }

        private void PropertyChangedLocationPropagator(object sender, PropertyChangedEventArgs e)
        {
            RefreshLayout();
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

        #region Equals

        public override bool Equals(object obj)
        {
            return (this == obj as LinkConnector);
        }

        #endregion // Equals

        #region GetHashCode

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^ Target.GetHashCode();
        }

        #endregion // GetHashCode

        #region operator ==

        public static bool operator == (LinkConnector x, LinkConnector y)
        {
            if (y == (object)null)
                return false;
            return x.Source == y.Source && x.Target == y.Target;
        }

        #endregion // operator ==

        #region operator !=

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(LinkConnector x, LinkConnector y)
        {
            return !(x == y);
        }

        #endregion // operator !=

        #region ToString

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Source.Name, Target.Name);
        }

        #endregion // ToString
    }
}
