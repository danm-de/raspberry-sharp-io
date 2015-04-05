using System;
using System.Windows.Input;

namespace Test.Components.Pcd8544.WpfGui.Commands
{
    public class DelegateCommand<T> : ICommand<T>
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute, Func<T,bool> canExecute) {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        bool ICommand.CanExecute(object parameter) {
            return ReferenceEquals(canExecute, null) 
                || canExecute((T)parameter);
        }

        void ICommand.Execute(object parameter) {
            if (ReferenceEquals(execute, null)) {
                return;
            }

            execute((T)parameter);
        }

        bool ICommand<T>.CanExecute(T parameter) {
            return ReferenceEquals(canExecute, null) 
                || canExecute(parameter);
        }

        void ICommand<T>.Execute(T parameter) {
            if (ReferenceEquals(execute, null)) {
                return;
            }

            execute(parameter);
        }

        public virtual void RaiseCanExecuteChanged() {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged() {
            var handler = CanExecuteChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void Execute(T parameter) {
            ((ICommand<T>) this).Execute(parameter);
        }
    }

    public class DelegateCommand : DelegateCommand<object> {
        public DelegateCommand(Action execute, Func<bool> canExecute) 
            :base(_ => execute(), _ => canExecute())
        {}

        public virtual void Execute() {
            Execute(null);
        }
    }
}