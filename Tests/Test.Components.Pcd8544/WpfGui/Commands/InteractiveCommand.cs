using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Test.Components.Pcd8544.WpfGui.Commands
{
    public class InteractiveCommand : TriggerAction<DependencyObject>
    {
        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", 
            typeof(ICommand), 
            typeof(InteractiveCommand), 
            new UIPropertyMetadata(null)
        );

        private string commandName;

        public string CommandName {
            get {
                ReadPreamble();
                return commandName;
            }
            set {
                if (commandName == value) {
                    return;
                }

                WritePreamble();
                commandName = value;
                WritePostscript();
            }
        }

        public ICommand Command {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void Invoke(object parameter) {
            if (AssociatedObject == null) {
                return;
            }

            var command = ResolveCommand();
            if (command == null) {
                return;
            }

            if (!command.CanExecute(parameter)) {
                return;
            }

            command.Execute(parameter);
        }

        private ICommand ResolveCommand() {
            if (Command != null) {
                return Command;
            }

            if (AssociatedObject == null) {
                return null;
            }

            var propertyInfos = AssociatedObject
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var info in propertyInfos) {
                if (typeof(ICommand).IsAssignableFrom(info.PropertyType) 
                    && string.Equals(info.Name, CommandName, StringComparison.Ordinal)) 
                {
                    return (ICommand)info.GetValue(AssociatedObject, null);
                }
            }

            return null;
        }
    }
}