using System.ComponentModel;
using System.Runtime.CompilerServices;
using Test.Components.Pcd8544.WpfGui.Properties;

namespace Test.Components.Pcd8544.WpfGui.Abstract
{
    public abstract class ObservableObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = "") {
            if (Equals(field, newValue)) {
                return;
            }

            field = newValue;
            
            OnPropertyChanged(propertyName);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}