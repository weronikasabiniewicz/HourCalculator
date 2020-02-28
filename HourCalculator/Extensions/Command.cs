using System;
using System.Diagnostics;
using System.Windows.Input;

namespace HourCalculator
{
    public class Command : ICommand
    {
        private readonly Predicate<object> _canExecute;

        private readonly Action<object> _execute;


        public Command(Action<object> execute)
            : this(execute, null)
        {
        }

        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameters)
        {
            return _canExecute?.Invoke(parameters) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameters)
        {
            _execute(parameters);
        }
    }
}