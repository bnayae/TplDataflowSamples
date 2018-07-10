#region Using

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

#endregion // Using

namespace Bnaya.Samples
{
    public class GenericCommand : ICommand
    {
        private bool _canExecute = true;
        private readonly Action _execute;

        public GenericCommand(string title, Action execute, Color backColor, Color foreColor)
        {
            _execute = execute;
            Title = title;
            BackColor = backColor.ToString();
            ForeColor = foreColor.ToString();
        }

        public string Title { get; private set; }

        public string ForeColor { get; private set; }
        public string BackColor { get; private set; }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged = (s, e) => { };
        
        public void SetCanExecute(bool value)
        {
            _canExecute = value;
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
