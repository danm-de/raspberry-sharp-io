using System.Windows.Input;

namespace Test.Components.Pcd8544.WpfGui.Commands
{
    public interface ICommand<in T> : ICommand
    {
        bool CanExecute(T parameter);
        void Execute(T parameter);
    }
}